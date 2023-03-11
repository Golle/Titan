using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.DBT;

public unsafe struct DEV_BROADCAST_DEVICEINTERFACE_A
{
    public uint dbcc_size;
    public uint dbcc_devicetype;
    public uint dbcc_reserved;
    public Guid dbcc_classguid;
    public fixed byte dbcc_name[1];
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public unsafe struct DEV_BROADCAST_DEVICEINTERFACE_W
{
    public uint dbcc_size;
    public uint dbcc_devicetype;
    public uint dbcc_reserved;
    public Guid dbcc_classguid;
    public fixed char dbcc_name[1];
}
