using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Titan.Tools.Core.Images.Aseprite;

#pragma warning disable CS0649

internal enum CelType : ushort
{
    Raw = 0,
    LinkedCel = 1,
    CompressedImage = 2,
    CompressedTilemap = 3
}

[Flags]
public enum LayerFlags
{
    Visible = 1,
    Editable = 2,
    LockMovement = 4,
    Background = 8,
    PreferLinkedCels = 16,
    LayerGroupCollapesed = 32,
    ReferenceLayer = 64
}

public enum BlendMode : ushort
{
    Normal = 0,
    Multiply = 1,
    Screen = 2,
    Overlay = 3,
    Darken = 4,
    Lighten = 5,
    ColorDodge = 6,
    ColorBurn = 7,
    HardLight = 8,
    SoftLight = 9,
    Difference = 10,
    Exclusion = 11,
    Hue = 12,
    Saturation = 13,
    Color = 14,
    Luminosity = 15,
    Addition = 16,
    Subtract = 17,
    Divide = 18,
}

internal enum AsepriteChunkType : ushort
{
    OldPalette = 0x0004,
    OldPaletteChunk = 0x0011,
    Layer = 0x2004,
    Cel = 0x2005,
    CelExtra = 0x2006,
    ColorProfile = 0x2007,
    ExternalFiles = 0x2008,
    Mask = 0x2016, // deprecated
    Path = 0x2017, // never used,
    Tags = 0x2018,
    Palette = 0x2019,
    UserData = 0x2020,
    Slice = 0x2022,
    Tileset = 0x2023
}

internal static class FLIConstants
{
    public const int FLI_MAGIC_NUMBER = 0xAF11;
    public const int FLC_MAGIC_NUMBER = 0xAF12;
    public const int FLI_FRAME_MAGIC_NUMBER = 0xF1FA;
    public const int FLI_COLOR_256_CHUNK = 4;
    public const int FLI_DELTA_CHUNK = 7;
    public const int FLI_COLOR_64_CHUNK = 11;
    public const int FLI_LC_CHUNK = 12;
    public const int FLI_BLACK_CHUNK = 13;
    public const int FLI_BRUN_CHUNK = 15;
    public const int FLI_COPY_CHUNK = 16;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal unsafe struct CelChunk
{
    public ushort LayerIndex;
    public short X;
    public short Y;
    public byte Opacity;
    public CelType Type;
    public fixed byte Reserved[7];
}

internal unsafe struct LayerChunk
{
    public LayerFlags Flags;
    public ushort Type;
    public ushort ChildLevel;
    public ushort Width; // ignored
    public ushort Height; // ignored
    public BlendMode BlendMode;
    public byte Opacity;
    public fixed byte Reserved[3];
    // If type == 2
    public uint TilesetIndex;
}

[DebuggerDisplay("[{X, Y}]")]
internal struct FixedPoint
{
    public short X, Y;
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
internal struct AsespriteString
{
    public ushort Size;
}

internal unsafe struct ColorPaletteChunk
{
    public uint PaletteSize;
    public uint FirstIndex;
    public uint LastIndex;
    public fixed byte Reserved[8];

}

internal struct Palette
{
    public ushort Flag;
    public byte R, G, B, A;

}

internal unsafe struct ColorProfileChunk
{
    public ushort Type;
    public ushort Flags;
    public FixedPoint FixedPoint;
    public fixed byte Reserved[8];
    // If type == ICC
    public uint IccProfileLength;
}

[StructLayout(LayoutKind.Sequential, Size = 128, Pack = 2)]
internal unsafe struct AsepriteHeader
{
    public uint FileSize;
    public ushort MagicNumber;
    public ushort Frames;
    public ushort Width;
    public ushort Height;
    public ushort ColorDepth;
    public uint Flags;
    public ushort Speed;
    public uint Zero0;
    public uint Zero1;
    public byte PaletteTransparent;
    public fixed byte Ignored[3];
    public ushort NumberOfColors;
    public byte PixelWidth;
    public byte PixelHeight;
    public short X;
    public short Y;
    public ushort GridWidth;
    public ushort GridHeight;
    public fixed byte FutureUse[84];
}

[StructLayout(LayoutKind.Sequential, Size = 16)]
internal unsafe struct AsepriteFrame
{
    public uint Size;
    public ushort MagicNumber;
    public ushort NumberOfChunksOld;
    public ushort FrameDurationMs;
    public fixed byte NotUsed[2];
    public uint NumberOfChunks;
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
internal struct AsepriteChunk
{
    public uint Size;
    public AsepriteChunkType Type;
}
