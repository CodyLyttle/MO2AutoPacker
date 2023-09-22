using System.Windows;

namespace MO2AutoPacker.UI.AttachedProperties;

public static partial class Dimensions
{
    public static readonly DependencyProperty MatchWidthToHeightProperty = DependencyProperty.RegisterAttached(
        "MatchWidthToHeight", typeof(bool), typeof(Dimensions), new PropertyMetadata(false, (d, e) =>
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
                    element.Width = element.ActualHeight;
            }
        }));

    public static bool GetMatchWidthToHeight(DependencyObject element) =>
        (bool) element.GetValue(MatchWidthToHeightProperty);

    public static void SetMatchWidthToHeight(DependencyObject element, bool value) =>
        element.SetValue(MatchWidthToHeightProperty, value);
}