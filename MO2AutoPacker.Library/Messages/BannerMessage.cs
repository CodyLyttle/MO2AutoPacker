namespace MO2AutoPacker.Library.Messages;

public class BannerMessage
{
    public enum Type
    {
        Error,
        Info,
        Success
    }
    
    public Type MessageType { get; }
    public string Message { get; }

    public BannerMessage(Type messageType, string message)
    {
        MessageType = messageType;
        Message = message;
    }
}