namespace Titan.Platform.Win32.Win32;

[Flags]
public enum RIDEV : uint
{
    RIDEV_REMOVE = 0x00000001,
    RIDEV_EXCLUDE = 0x00000010,
    RIDEV_PAGEONLY = 0x00000020,
    RIDEV_NOLEGACY = 0x00000030,
    RIDEV_INPUTSINK = 0x00000100,
    RIDEV_CAPTUREMOUSE = 0x00000200, // effective when mouse nolegacy is specified, otherwise it would be an error
    RIDEV_NOHOTKEYS = 0x00000200, // effective for keyboard.
    RIDEV_APPKEYS = 0x00000400, // effective for keyboard.
    RIDEV_EXINPUTSINK = 0x00001000,
    RIDEV_DEVNOTIFY = 0x00002000
}
