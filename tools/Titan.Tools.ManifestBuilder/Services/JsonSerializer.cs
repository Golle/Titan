using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Titan.Tools.ManifestBuilder.Services;
internal interface IJsonSerializer
{
    T? Deserialize<T>(ReadOnlySpan<byte> bytes);
    ValueTask<T?> DeserializeAsync<T>(Stream utf8Stream);
    Task SerializeAsync<T>(Stream stream, in T value, bool writeIndented = false);
    string Serialize<T>(in T value);
    byte[] SerializeToUtf8<T>(in T value, bool writeIndented = false);
}
public class JsonSerializer : IJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    private static readonly JsonSerializerOptions OptionsPrettyPrint = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public T? Deserialize<T>(ReadOnlySpan<byte> bytes) => System.Text.Json.JsonSerializer.Deserialize<T>(bytes, Options);
    public ValueTask<T?> DeserializeAsync<T>(Stream utf8Stream) => System.Text.Json.JsonSerializer.DeserializeAsync<T>(utf8Stream, Options);
    public Task SerializeAsync<T>(Stream stream, in T value, bool writeIndented = false) => System.Text.Json.JsonSerializer.SerializeAsync(stream, value, writeIndented ? OptionsPrettyPrint : Options);
    public string Serialize<T>(in T value) => System.Text.Json.JsonSerializer.Serialize(value, Options);
    public byte[] SerializeToUtf8<T>(in T value, bool writeIndented)
    {
        using var stream = new MemoryStream();
        System.Text.Json.JsonSerializer.Serialize(stream, value, writeIndented ? OptionsPrettyPrint : Options);
        return stream.ToArray();
    }
}
