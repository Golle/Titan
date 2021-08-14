using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Titan.Windows.Win32
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public readonly struct HFONT
    {
        private readonly int _handle;

        public HFONT(int handle) => _handle = handle;
        public static implicit operator HGDIOBJ(in HFONT font) => new(font._handle);

        public static implicit operator HFONT(in HGDIOBJ obj) => new(obj.Handle);
    }
}
