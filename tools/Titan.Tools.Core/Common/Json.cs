using System.Text.Json;

namespace Titan.Tools.Core.Common;

#pragma warning disable IL2026 //NOTE(Jens): disable the warning about types with trimmable. Fix it when/if it's a problem. 
public static class Json
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

    public static T? Deserialize<T>(ReadOnlySpan<byte> bytes) => JsonSerializer.Deserialize<T>(bytes, Options);
    public static ValueTask<T?> DeserializeAsync<T>(Stream utf8Stream) => JsonSerializer.DeserializeAsync<T>(utf8Stream, Options);
    public static Task SerializeAsync<T>(Stream stream, in T value, bool writeIndented = false) => JsonSerializer.SerializeAsync(stream, value, writeIndented ? OptionsPrettyPrint : Options);
    public static string Serialize<T>(in T value) => JsonSerializer.Serialize(value, Options);
    public static byte[] SerializeToUtf8<T>(in T value, bool writeIndented)
    {
        using var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, value, writeIndented ? OptionsPrettyPrint : Options);
        return stream.ToArray();
    }
}

