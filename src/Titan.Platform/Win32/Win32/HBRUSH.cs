using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Titan.Platform.Win32.Win32
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = sizeof(int))]
    public readonly struct HBRUSH
    {
        private readonly int _handle;
        
        public static implicit operator HGDIOBJ(in HBRUSH brush) => new(brush._handle);
    }
}
