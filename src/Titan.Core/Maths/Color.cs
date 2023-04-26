using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Memory;

namespace Titan.Core.Maths;

//NOTE(Jens): maybe we should have a color with an uint, and a ColorF for float values. This will occupy 16 bytes, but 4 bytes would be enough.

public struct Color
{
    private const float ByteMax = byte.MaxValue;
    public float R, G, B, A;
    public Color(float r, float g, float b, float a = 1.0f)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public Color(uint rgba)
    {
        A = (rgba & 0xff) / ByteMax;
        B = ((rgba >> 8) & 0xff) / ByteMax;
        G = ((rgba >> 16) & 0xff) / ByteMax;
        R = ((rgba >> 24) & 0xff) / ByteMax;
    }

    public Color(byte r, byte g, byte b, byte a = byte.MaxValue)
    {
        R = r / ByteMax;
        G = g / ByteMax;
        B = b / ByteMax;
        A = a / ByteMax;
    }

    public static Color FromRGBA(uint rgba) => new(rgba);
    public static Color FromRGB(uint rgb)
    {
        var rgba = rgb << 8 | 0xff;
        return new(rgba);
    }

    public static Color From(ReadOnlySpan<float> colors)
    {
        Unsafe.SkipInit(out Color color);
        unsafe
        {
            MemoryUtils.Copy(&color, colors);
        }
        return color;
    }

    public static readonly Color White = new(1f, 1f, 1f);
    public static readonly Color Red = new(1f, 0f, 0f);
    public static readonly Color Green = new(0f, 1f, 0f);
    public static readonly Color Blue = new(0f, 0f, 1f);
    public static readonly Color Magenta = new(1f, 0f, 1f);
    public static readonly Color Yellow = new(1f, 1f, 0f);
    public static readonly Color Black = new(0f, 0f, 0f);
    public static readonly Color Transparent = new(0f, 0f, 0f, 0f);
}
