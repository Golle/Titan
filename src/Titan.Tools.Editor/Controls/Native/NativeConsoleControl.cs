using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Platform;
using Titan.Platform.Win32;

namespace Titan.Tools.Editor.Controls.Native;

public class NativeConsoleControl : NativeControlHost
{
    private const int GWL_STYLE = -16;
    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new PlatformHandle(0, "Platform not supported.");
        }

        Kernel32.AllocConsole();
        var window = Kernel32.GetConsoleWindow();

        var style = (long)USER32.GetWindowLongPtrW(window, GWL_STYLE);
        //NOTE(Jens): Just remove anything that might interfer with the host
        var newStyle = style & (long)~(WINDOWSTYLES.WS_BORDER | WINDOWSTYLES.WS_CAPTION | WINDOWSTYLES.WS_SYSMENU | WINDOWSTYLES.WS_DLGFRAME | WINDOWSTYLES.WS_CLIPSIBLINGS | WINDOWSTYLES.WS_OVERLAPPEDWINDOW | WINDOWSTYLES.WS_POPUPWINDOW);
        USER32.SetWindowLongPtrW(window, GWL_STYLE, (nint)newStyle);

        return new ConsoleHandle
        {
            Handle = window,
            HandleDescriptor = "Windows Console"
        };
    }

    private class ConsoleHandle : INativeControlHostDestroyableControlHandle
    {
        public required nint Handle { get; init; }
        public string? HandleDescriptor { get; init; }
        public void Destroy() => Kernel32.FreeConsole();
    }
}
