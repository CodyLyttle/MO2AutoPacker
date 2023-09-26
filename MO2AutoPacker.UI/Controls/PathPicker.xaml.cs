using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MO2AutoPacker.UI.Controls;

public partial class PathPicker : UserControl
{
    public PathPicker()
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
        nameof(Text), typeof(string), typeof(PathPicker),
        new PropertyMetadata( /* Use default value from the UI bound control */));

    #endregion

    #region DependencyProperty Watermark

    public string Watermark
    {
        get => (string) GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
        nameof(Watermark), typeof(string), typeof(PathPicker),
        new PropertyMetadata(GetDefaultWatermarkValue( /* Use default value from the UI bound control */)));

    private static string GetDefaultWatermarkValue() => "Watermark";

    #endregion

    #region DependencyProperty Command

    public ICommand Command
    {
        get => (ICommand) GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command), typeof(ICommand), typeof(PathPicker),
        new PropertyMetadata());

    #endregion
}