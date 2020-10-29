using System.Text.Json;
using System.Text.Json.Serialization;

namespace Titan.Core.Common
{
    internal class JsonSerializerWrapper : IJsonSerializer
    {

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            IncludeFields = true,
            IgnoreReadOnlyFields =  false,
            IgnoreReadOnlyProperties = false,
            Converters = { new JsonStringEnumConverter()}
        };
        public T Deserialize<T>(string json)
        {
            
            return JsonSerializer.Deserialize<T>(json, _options);
        }

        public string Serialize<T>(in T obj)
        {
            return JsonSerializer.Serialize(obj, _options);
        }
    }
}
