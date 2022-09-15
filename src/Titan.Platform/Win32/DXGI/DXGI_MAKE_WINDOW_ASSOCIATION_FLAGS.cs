namespace Titan.Platform.Win32.DXGI;

[Flags]
public enum DXGI_MAKE_WINDOW_ASSOCIATION_FLAGS
{
    DXGI_MWA_NO_WINDOW_CHANGES = (1 << 0),
    DXGI_MWA_NO_ALT_ENTER = (1 << 1),
    DXGI_MWA_NO_PRINT_SCREEN = (1 << 2),
    DXGI_MWA_VALID = (0x7)
}
