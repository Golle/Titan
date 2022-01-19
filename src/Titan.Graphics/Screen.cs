using System.Diagnostics;
using Titan.Windows.Win32;
using static Titan.Windows.Win32.SystemMetricsCode;

namespace Titan.Graphics;

public static class Screen
{
    public static Size GetPrimaryMonitorSize()
    {
        var height = User32.GetSystemMetrics(SM_CYSCREEN);
        var width = User32.GetSystemMetrics(SM_CXSCREEN);

        Debug.Assert(height != 0, "Failed to get the height of the screen");
        Debug.Assert(width != 0, "Failed to get the width of the screen");

        return new Size(width, height);
    }

    public static uint GetNumberOfMonitors()
    {
        var monitors = User32.GetSystemMetrics(SM_CMONITORS);
        Debug.Assert(monitors != 0, "Failed to get the number of monitors");
        return (uint)monitors;
    }
}
