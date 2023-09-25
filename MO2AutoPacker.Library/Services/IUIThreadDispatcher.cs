namespace MO2AutoPacker.Library.Services;

public interface IUIThreadDispatcher
{
    void Invoke(Action action);
}