namespace Titan.Platform.Win32.DBT;

public enum DBT_DEVICE_TYPES : uint
{
    DBT_NO_DISK_SPACE = 0x0047,
    DBT_LOW_DISK_SPACE = 0x0048,
    DBT_CONFIGMGPRIVATE = 0x7FFF,
    DBT_DEVICEARRIVAL = 0x8000,  // system detected a new device
    DBT_DEVICEQUERYREMOVE = 0x8001,  // wants to remove, may fail
    DBT_DEVICEQUERYREMOVEFAILED = 0x8002,  // removal aborted
    DBT_DEVICEREMOVEPENDING = 0x8003,  // about to remove, still avail.
    DBT_DEVICEREMOVECOMPLETE = 0x8004,  // device is gone
    DBT_DEVICETYPESPECIFIC = 0x8005,  // type specific event
    DBT_CUSTOMEVENT = 0x8006,  // user-defined event
    DBT_DEVTYP_OEM = 0x00000000,  // oem-defined device type
    DBT_DEVTYP_DEVNODE = 0x00000001,  // devnode number
    DBT_DEVTYP_VOLUME = 0x00000002,  // logical volume
    DBT_DEVTYP_PORT = 0x00000003,  // serial, parallel
    DBT_DEVTYP_NET = 0x00000004,  // network resource
    DBT_DEVTYP_DEVICEINTERFACE = 0x00000005,  // device interface class
    DBT_DEVTYP_HANDLE = 0x00000006,  // file system handle
}
