namespace MO2AutoPacker.Library.UIAbstractions
{
    public interface IUIThreadDispatcher
    {
        void Invoke(Action action);
    }
}