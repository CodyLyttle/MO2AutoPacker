using System;
using System.Windows;
using MO2AutoPacker.Library.ViewModels;

namespace MO2AutoPacker.UI.AttachedProperties;

// Allows xaml view model injection.
// This is particularly useful for binding built-in controls to a viewmodel without having to create a custom-control.
public static class ViewModelProviderAttachedProperty
{
    public static readonly DependencyProperty ViewModelProviderProperty = DependencyProperty.RegisterAttached(
        "ViewModelProvider", typeof(Type), typeof(ViewModelProviderAttachedProperty), new PropertyMetadata(null,
            (d, e) =>
            {
                if (d is not FrameworkElement)
                    return;

                var vmType = (Type) e.NewValue;
                if (!typeof(ViewModelBase).IsAssignableFrom((Type) e.NewValue))
                    return;

                d.SetValue(FrameworkElement.DataContextProperty, App.Current.Services.GetService(vmType));
            }));

    public static void SetViewModelProvider(DependencyObject element, Type? value) =>
        element.SetValue(ViewModelProviderProperty, value);

    public static Type? GetViewModelProvider(DependencyObject element) =>
        (Type?) element.GetValue(ViewModelProviderProperty);
}