using System;

namespace Titan.Graphics.Modules;

internal unsafe struct WindowFunctions
{
    public delegate*<ref nint, in WindowDescriptor, WindowEventQueue*, bool> CreateWindow;
    public delegate*<ref nint, bool> DestroyWindow;
    public delegate*<nint, bool> Show;
    public delegate*<nint, bool> Hide;
    public delegate*<nint, ReadOnlySpan<char>, bool> SetTitle;
    public delegate*<nint, bool> Update;
    public delegate*<nint, out Point, bool> GetRelativeCursorPosition;
    public static WindowFunctions Create<T>() where T : IWindowFunctions =>
        new()
        {
            CreateWindow = &T.CreateWindow,
            SetTitle = &T.SetTitle,
            DestroyWindow = &T.DestroyWindow,
            Hide = &T.Hide,
            Show = &T.Show,
            Update = &T.Update,
            GetRelativeCursorPosition =  &T.GetRelativeCursorPosition
        };
}
