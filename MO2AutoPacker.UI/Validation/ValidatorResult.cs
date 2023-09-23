namespace MO2AutoPacker.UI.Validation;

public class ValidatorResult
{
    public bool WasSuccessful { get; }
    public string Message { get; }

    private ValidatorResult(bool wasSuccessful, string message)
    {
        WasSuccessful = wasSuccessful;
        Message = message;
    }

    public static ValidatorResult Success() => new(true, string.Empty);

    public static ValidatorResult Fail(string message) => new(false, message);
}