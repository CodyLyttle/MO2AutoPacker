using System.Windows.Controls;
using MO2AutoPacker.Library.ViewModels;

namespace MO2AutoPacker.UI.Views;

public partial class ModListManager : UserControl
{
    public ModListManager()
    {
        InitializeComponent();
        DataContext = ViewModelProvider.GetViewModel<ModListManagerViewModel>();
    }
}