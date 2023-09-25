namespace MO2AutoPacker.Library.Validation;

public class ValidatorResult
{
    private ValidatorResult(bool wasSuccessful, string message)
    {
        WasSuccessful = wasSuccessful;
        Message = message;
    }

    public bool WasSuccessful { get; }
    public string Message { get; }

    public static ValidatorResult Success() => new(true, string.Empty);

    public static ValidatorResult Fail(string message) => new(false, message);
}