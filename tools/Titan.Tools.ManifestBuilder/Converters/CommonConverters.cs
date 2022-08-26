using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Titan.Tools.ManifestBuilder.Converters;

public class EmptyStringFallbackConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(value as string))
        {
            return parameter as string;
        }
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
