using System;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.D3D11;

[Flags]
public enum D3D11_RLDO_FLAGS
{
    D3D11_RLDO_SUMMARY = 0x1,
    D3D11_RLDO_DETAIL = 0x2,
    D3D11_RLDO_IGNORE_INTERNAL = 0x4,
}
