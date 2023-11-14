using System;
using MO2AutoPacker.Library.Services;

namespace MO2AutoPacker.UI.Implementations;

public class WpfDispatcher : IUIThreadDispatcher
{
    public void Invoke(Action callback) => App.Current.Dispatcher.Invoke(callback);

    public TResult Invoke<TResult>(Func<TResult> callback) => App.Current.Dispatcher.Invoke(callback);
}