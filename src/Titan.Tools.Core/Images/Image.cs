using Titan.Platform.Win32.DXGI;

namespace Titan.Tools.Core.Images;

public record Image
{
    public required byte[] Data { get; init; }
    public required uint BitsPerPixel { get; init; }
    public required uint Width { get; init; }
    public required uint Height { get; init; }
    public required DXGI_FORMAT Format { get; init; }
    public required uint Stride { get; init; }
    public int Size => Data.Length;
}
