using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using MO2AutoPacker.Library;
using MO2AutoPacker.Library.ViewModels;

namespace MO2AutoPacker.UI.Views;

public partial class ModListManager : UserControl
{
    public ModListManager()
    {
        InitializeComponent();
        DataContext = Services.Provider.GetService<ModListManagerViewModel>();
    }
}