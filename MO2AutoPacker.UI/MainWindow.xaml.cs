using System.Windows;
using MO2AutoPacker.Library.ViewModels;

namespace MO2AutoPacker.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewmodel)
        {
            InitializeComponent();
            DataContext = viewmodel;
        }
    }
}