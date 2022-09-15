using Avalonia.Data.Converters;

namespace Titan.Tools.ManifestBuilder.Converters;

public static class DateTimeConverters
{
    public static readonly IValueConverter DateTimeToString = new FuncValueConverter<DateTime?, string>(date => date?.ToString("yyyy-MM-dd HH:MM:ss") ?? string.Empty);
}
