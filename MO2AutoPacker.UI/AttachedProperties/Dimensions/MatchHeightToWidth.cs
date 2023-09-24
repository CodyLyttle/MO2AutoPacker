using System.Windows;

namespace MO2AutoPacker.UI.AttachedProperties;

public static partial class Dimensions
{
    public static readonly DependencyProperty MatchHeightToWidthProperty = DependencyProperty.RegisterAttached(
        "MatchHeightToWidth", typeof(bool), typeof(Dimensions), new PropertyMetadata(false, (d, e) =>
        {
            if (d is not FrameworkElement element || e.NewValue is not bool shouldBind)
                return;

            if (shouldBind)
                element.SizeChanged += OnChanged;
            else
                element.SizeChanged -= OnChanged;

            static void OnChanged(object sender, SizeChangedEventArgs e)
            {
                if (sender is FrameworkElement element)
                    element.Height = element.Width;
            }
        }));

    public static bool GetMatchHeightToWidth(DependencyObject element) =>
        (bool) element.GetValue(MatchHeightToWidthProperty);

    public static void SetMatchHeightToWidth(DependencyObject element, bool value) =>
        element.SetValue(MatchHeightToWidthProperty, value);
}