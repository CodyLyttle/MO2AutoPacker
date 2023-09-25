using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MO2AutoPacker.Library.Messages;

namespace MO2AutoPacker.UI.Converters;

public class BannerMessageTypeToColorConverter : IValueConverter
{
    private static readonly SolidColorBrush ErrorBrush = new(Colors.IndianRed);
    private static readonly SolidColorBrush InfoBrush = new(Colors.LightSkyBlue);
    private static readonly SolidColorBrush SuccessBrush = new(Colors.ForestGreen);
    private static readonly SolidColorBrush FallbackBrush = new(Colors.Transparent);

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is BannerMessage.Type messageType)
            return messageType switch
            {
                BannerMessage.Type.Error => ErrorBrush,
                BannerMessage.Type.Info => InfoBrush,
                BannerMessage.Type.Success => SuccessBrush,
                _ => FallbackBrush
            };

        return FallbackBrush;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException($"{nameof(BannerMessageTypeToColorConverter)} is a one-way converter");
}