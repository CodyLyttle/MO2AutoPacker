using MO2AutoPacker.Library;
using MO2AutoPacker.Library.ViewModels;

namespace MO2AutoPacker.UI;

public static class ViewModelProvider
{
    public static T GetViewModel<T>() where T : ViewModelBase => ServiceProvider.GetService<T>();
}