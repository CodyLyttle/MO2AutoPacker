using System;
using System.Windows.Threading;
using MO2AutoPacker.Library.UIAbstractions;

namespace MO2AutoPacker.UI.Implementations;

public class WpfDispatcher : IUIThreadDispatcher
{
    public void Invoke(Action action)
    {
        Dispatcher.CurrentDispatcher.Invoke(action);
    }
}