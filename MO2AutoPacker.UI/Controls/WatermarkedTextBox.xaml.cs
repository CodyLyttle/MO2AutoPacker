using System.Windows;
using System.Windows.Controls;

namespace MO2AutoPacker.UI.Controls;

public partial class WatermarkedTextBox : UserControl
{
    public WatermarkedTextBox()
    {
        InitializeComponent();
    }

    #region DependencyProperty Text

    public string Text
    {
        get => (string) GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        "Text", typeof(string), typeof(WatermarkedTextBox),
        new PropertyMetadata(GetDefaultTextValue()));

    private static string GetDefaultTextValue()
    {
        return string.Empty;
    }

    #endregion

    #region DependencyProperty Watermark

    public string Watermark
    {
        get => (string) GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
        "Watermark", typeof(string), typeof(WatermarkedTextBox),
        new PropertyMetadata(GetDefaultWatermarkValue()));

    private static string GetDefaultWatermarkValue()
    {
        return "Watermark";
    }

    #endregion

    #region DependencyProperty WatermarkOpacity

    public double WatermarkOpacity
    {
        get => (double) GetValue(WatermarkOpacityProperty);
        set => SetValue(WatermarkOpacityProperty, value);
    }

    public static readonly DependencyProperty WatermarkOpacityProperty = DependencyProperty.Register(
        "WatermarkOpacity", typeof(double), typeof(WatermarkedTextBox),
        new PropertyMetadata(GetDefaultWatermarkOpacityValue()));

    private static double GetDefaultWatermarkOpacityValue()
    {
        return 0.66;
    }

    #endregion
}