using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MO2AutoPacker.Library.ViewModels;

namespace MO2AutoPacker.UI.Views;

public partial class Banner : UserControl
{
    private readonly BannerViewModel _viewmodel;
    private readonly DoubleAnimation _fadeAnimation = new();

    public Banner()
    {
        InitializeComponent();
        _viewmodel = ViewModelProvider.GetViewModel<BannerViewModel>();
        _viewmodel.PropertyChanged += OnPropertyChanged;
        DataContext = _viewmodel;
        Opacity = GetTargetOpacity();
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(BannerViewModel.ShowMessage))
            return;

        StartOpacityAnimation(GetTargetOpacity());
    }

    private double GetTargetOpacity()
    {
        return _viewmodel.ShowMessage ? 1 : 0;
    }
    
    private void StartOpacityAnimation(double opacity)
    {
        Dispatcher.Invoke(() =>
        {
            _fadeAnimation.To = opacity;
            _fadeAnimation.Duration = _viewmodel.FadeDuration;
            BeginAnimation(OpacityProperty, _fadeAnimation);
        });
    }
}