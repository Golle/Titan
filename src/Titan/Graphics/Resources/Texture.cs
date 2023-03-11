using System.Diagnostics;

namespace Titan.Graphics.Resources;

[DebuggerDisplay("Width: {Width} Height: {Height} Format: {Format}")]
public struct Texture
{
    public uint Width;
    public uint Height;
    public TextureFormat Format;
}
