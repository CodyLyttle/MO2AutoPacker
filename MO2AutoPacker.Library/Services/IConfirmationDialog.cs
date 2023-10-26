namespace MO2AutoPacker.Library.Services;

public interface IConfirmationDialog
{
    bool PromptUser(string title, string caption);
}