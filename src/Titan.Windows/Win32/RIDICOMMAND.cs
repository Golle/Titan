namespace Titan.Windows.Win32;

public enum RIDICOMMAND : uint
{
    RIDI_PREPARSEDDATA = 0x20000005,
    RIDI_DEVICENAME = 0x20000007,  // the return valus is the character length, not the byte size
    RIDI_DEVICEINFO = 0x2000000b,
}
