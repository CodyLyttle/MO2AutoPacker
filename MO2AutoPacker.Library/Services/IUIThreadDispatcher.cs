namespace MO2AutoPacker.Library.Services;

public interface IUIThreadDispatcher
{
    void Invoke(Action callback);

    TResult Invoke<TResult>(Func<TResult> callback);
}