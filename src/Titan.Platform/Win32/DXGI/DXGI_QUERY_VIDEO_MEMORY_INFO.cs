namespace Titan.Platform.Win32.DXGI;

public struct DXGI_QUERY_VIDEO_MEMORY_INFO
{
    public ulong Budget;
    public ulong CurrentUsage;
    public ulong AvailableForReservation;
    public ulong CurrentReservation;
}
