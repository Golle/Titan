using Titan.Core.Maths;

namespace Titan.Windows;
public record struct WindowCreationArgs(uint Width, uint Height, string Title, bool Windowed = true, bool Resizable = true, bool AlwaysOnTop = false);
public interface IWindow
{
    WindowHandle Handle { get; }
    uint Width { get; }
    uint Height { get; }
    Size Size { get; }
    Point GetRelativeCursorPosition();
    void SetTitle(string title);
    bool Update();
}
