using System;
// ReSharper disable InconsistentNaming

namespace Titan.Windows.D3D11;

[Flags]
public enum D3D11_CLEAR_FLAG : uint
{
    D3D11_CLEAR_DEPTH = 0x1u,
    D3D11_CLEAR_STENCIL = 0x2u
}
