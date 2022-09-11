using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.Win32;

[StructLayout(LayoutKind.Explicit)]
public struct RAWINPUT
{
    [FieldOffset(0)]
    public RAWINPUTHEADER Header;
    
    //NOTE(Jens): Hardcoded size 24, this is on a x64 architecture. We might be able to replace this with a property and an unsafe cast on an offset.
    [FieldOffset(24)] 
    public RAWHID Hid;
    //[FieldOffset(24)]
    //public RAWMOUSE mouse;
    //[FieldOffset(24)]
    //public RAWKEYBOARD keyboard;
}
