using System.Windows.Input;
using MO2AutoPacker.Library.Services;

namespace MO2AutoPacker.Library;

public class CancellableCommand : ICommand
{
    private readonly Func<CancellationToken, Task> _command;
    private readonly IUIThreadDispatcher _dispatcher;
    private readonly object _lock = new();
    private Func<object?, bool>? _canBegin;

    private CancellationTokenSource? _cts;
    private bool _hasFailed;
    private bool _isCancelling;
    private bool _isRunning;

    public CancellableCommand(IUIThreadDispatcher dispatcher, Func<CancellationToken, Task> command)
    {
        _dispatcher = dispatcher;
        _command = command;
    }

    public event EventHandler? CanExecuteChanged;

    public event EventHandler<bool>? Completed;

    public event EventHandler<Exception>? ErrorOccurred;

    public Func<object?, bool>? CanBegin
    {
        get => _canBegin;
        set
        {
            if (value == _canBegin)
                return;

            _canBegin = value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool HasFailed
    {
        get => _hasFailed;
        private set
        {
            if (value == _hasFailed)
                return;

            _hasFailed = value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsCancelling
    {
        get => _isCancelling;
        private set
        {
            if (value == _isCancelling)
                return;

            _isCancelling = value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }


    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            if (value == _isRunning)
                return;

            _isRunning = value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool CanExecute(object? parameter)
    {
        if (HasFailed || IsCancelling)
            return false;

        // Can cancel running task.
        if (IsRunning)
            return true;

        // Can run task.
        return CanBegin is null || CanBegin.Invoke(parameter);
    }

    public void Execute(object? parameter)
    {
        lock (_lock)
        {
            if (IsCancelling)
                return;

            if (IsRunning)
            {
                IsCancelling = true;
                _cts!.Cancel();
            }

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    await _command.Invoke(token);
                    Completed?.Invoke(this, true);
                }
                catch (TaskCanceledException)
                {
                    _dispatcher.Invoke(() => Completed?.Invoke(this, false));
                }
                catch (Exception ex)
                {
                    _dispatcher.Invoke(() => ErrorOccurred?.Invoke(this, ex));
                    HasFailed = true;
                }

                _cts.Dispose();
                _dispatcher.Invoke(() =>
                {
                    IsRunning = false;
                    IsCancelling = false;
                });
            }, token);
        }
    }

    public void Cancel()
    {
        lock (_lock)
        {
            if (!IsRunning)
                return;

            _cts!.Cancel();
        }
    }

    public void ResetFailedCommand()
    {
        if (HasFailed)
        {
            HasFailed = false;
        }
    }
}