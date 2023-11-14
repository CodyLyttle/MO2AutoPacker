using System;
using System.Windows.Threading;
using MO2AutoPacker.Library.Services;

namespace MO2AutoPacker.UI.Implementations;

public class WpfDispatcher : IUIThreadDispatcher
{
    public void Invoke(Action action) => App.Current.Dispatcher.Invoke(action);
}