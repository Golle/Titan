using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Titan.Windows.Win32
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public readonly struct HGDIOBJ
    {
        private readonly int _handle;
        public int Handle => _handle;
        public HGDIOBJ(int handle) => _handle = handle;
    }
}
