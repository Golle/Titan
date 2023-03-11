namespace Titan.Platform.Win32;

public unsafe struct CREATESTRUCTA
{
    public void* lpCreateParams;
    public nint hInstance;
    public nint hMenu;
    public HWND hwndParent;
    public int cy;
    public int cx;
    public int y;
    public int x;
    public int style;
    public sbyte* lpszName;
    public sbyte* lpszClass;
    public uint dwExStyle;
}
