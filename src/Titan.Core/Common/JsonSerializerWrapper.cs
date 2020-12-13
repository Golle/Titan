using System.Text.Json;
using System.Text.Json.Serialization;

namespace Titan.Core.Common
{
    internal class JsonSerializerWrapper : IJsonSerializer
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            IncludeFields = true,
            IgnoreReadOnlyFields =  false,
            IgnoreReadOnlyProperties = false,
            Converters = { new JsonStringEnumConverter()}
        };
        public T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);
        public string Serialize<T>(in T obj) => JsonSerializer.Serialize(obj, Options);
    }
}
