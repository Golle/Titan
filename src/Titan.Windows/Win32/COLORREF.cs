using System.Runtime.InteropServices;

namespace Titan.Windows.Win32
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct COLORREF
    {
        private unsafe fixed byte _color[4];
        public unsafe COLORREF(byte r, byte g, byte b)
        {
            _color[0] = r;
            _color[1] = g;
            _color[2] = b;
        }


    }
}
