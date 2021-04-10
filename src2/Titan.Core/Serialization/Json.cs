using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Titan.Core.Serialization
{
    public static class Json
    {
        private static JsonSerializerOptions _options = new JsonSerializerOptions
        {
            IncludeFields = true,
            IgnoreReadOnlyFields = false,
            IgnoreReadOnlyProperties = false,
            Converters = { new JsonStringEnumConverter() }
        };

        public static T Deserialize<T>(ReadOnlySpan<byte> json) => JsonSerializer.Deserialize<T>(json, _options);
        public static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _options);
        public static string Serialize<T>(in T value) => JsonSerializer.Serialize(value);
    }
}
