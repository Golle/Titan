using Titan.Core;
using Titan.Core.Memory;

namespace Titan.Graphics.Modules;

public struct WindowDescriptor : IDefault<WindowDescriptor>
{
    private const int DefaultHeight = 768;
    private const int DefaultWidth = 1024;
    private const int MaxTitleLength = 128;

    public uint Width;
    public uint Height;
    public bool Resizable;
    private unsafe fixed char _title[MaxTitleLength];
    private int _titleLength;
    public unsafe ReadOnlySpan<char> Title
    {
        readonly get
        {
            fixed (char* pTitle = _title)
            {
                return new(pTitle, _titleLength);
            }
        }
        set
        {
            _titleLength = Math.Min(MaxTitleLength, value.Length);
            fixed (char* pSource = value)
            fixed (char* pDestination = _title)
            {
                MemoryUtils.Copy(pDestination, pSource, _titleLength * sizeof(char));
            }
        }
    }

    public static WindowDescriptor Default => new()
    {
        Height = DefaultHeight,
        Width = DefaultWidth,
        Resizable = true,
        Title = "n/a"
    };
}
