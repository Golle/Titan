namespace Titan.Platform.Win32.DXGI;


// add more errors from https://docs.microsoft.com/en-us/windows/win32/direct3ddxgi/dxgi-error
public static class DXGI_ERROR
{
    public const int NONE = 0;
    public const int DXGI_ERROR_NOT_FOUND = unchecked((int)0x887A0002);
}
