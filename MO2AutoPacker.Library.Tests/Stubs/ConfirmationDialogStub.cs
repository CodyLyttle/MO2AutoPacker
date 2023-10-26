using MO2AutoPacker.Library.Services;

namespace MO2AutoPacker.Library.Tests.Stubs;

public class ConfirmationDialogStub : IConfirmationDialog
{
    public ConfirmationDialogStub(bool result)
    {
        Result = result;
    }

    public bool Result { get; set; }

    public bool PromptUser(string title, string description) => Result;
}