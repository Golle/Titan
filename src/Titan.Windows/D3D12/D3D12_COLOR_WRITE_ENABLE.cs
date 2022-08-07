using System;

namespace Titan.Windows.D3D12;

[Flags]
public enum D3D12_COLOR_WRITE_ENABLE : byte
{
    D3D12_COLOR_WRITE_ENABLE_RED = 1,
    D3D12_COLOR_WRITE_ENABLE_GREEN = 2,
    D3D12_COLOR_WRITE_ENABLE_BLUE = 4,
    D3D12_COLOR_WRITE_ENABLE_ALPHA = 8,
    D3D12_COLOR_WRITE_ENABLE_ALL = (((D3D12_COLOR_WRITE_ENABLE_RED | D3D12_COLOR_WRITE_ENABLE_GREEN) | D3D12_COLOR_WRITE_ENABLE_BLUE) | D3D12_COLOR_WRITE_ENABLE_ALPHA)
}