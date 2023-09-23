namespace MO2AutoPacker.Library.Abstractions.UI
{
    public interface IUIThreadDispatcher
    {
        void Invoke(Action action);
    }
}