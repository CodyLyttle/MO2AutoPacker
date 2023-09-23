using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Microsoft.Extensions.DependencyInjection;
using MO2AutoPacker.Library;
using MO2AutoPacker.Library.ViewModels;

namespace MO2AutoPacker.UI.Views;

public partial class Banner : UserControl
{
    private readonly BannerViewModel _viewmodel;
    private readonly DoubleAnimation _fadeAnimation = new();

    public Banner()
    {
        InitializeComponent();
        _viewmodel = Services.Provider.GetService<BannerViewModel>()
                     ?? throw new InvalidOperationException(
                         $"Service provider is missing dependency '{nameof(BannerViewModel)}");
        
        DataContext = _viewmodel;
        Opacity = GetTargetOpacity();
        _viewmodel.PropertyChanged += OnPropertyChanged;
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