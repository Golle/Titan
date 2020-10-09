using System;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.Win32.D3D11
{
    [Flags]
    public enum D3D11_CPU_ACCESS_FLAG : uint
    {
        UNSPECIFIED = 0,
        D3D11_CPU_ACCESS_WRITE = 0x10000,
        D3D11_CPU_ACCESS_READ = 0x20000
    }
}
