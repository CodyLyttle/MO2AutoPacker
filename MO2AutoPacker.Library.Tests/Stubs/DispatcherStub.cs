using MO2AutoPacker.Library.Services;

namespace MO2AutoPacker.Library.Tests.Stubs;

public class DispatcherStub : IUIThreadDispatcher
{
    public void Invoke(Action action) => action.Invoke();
}