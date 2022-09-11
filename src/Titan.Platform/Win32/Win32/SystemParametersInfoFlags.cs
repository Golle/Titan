namespace Titan.Platform.Win32.Win32;

[Flags]
public enum SystemParametersInfoFlags
{
    SPIF_UPDATEINIFILE = 0x0001,
    SPIF_SENDWININICHANGE = 0x0002,
    SPIF_SENDCHANGE = SPIF_SENDWININICHANGE
}
