namespace MO2AutoPacker.Library.Messages;

public class LogMessage
{
    public LogMessage(string message)
    {
        Message = message;
    }

    public string Message { get; }
}