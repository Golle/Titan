namespace Titan.Windows.D3D12;

public enum D3D12_FILTER
{
    D3D12_FILTER_MIN_MAG_MIP_POINT = 0,
    D3D12_FILTER_MIN_MAG_POINT_MIP_LINEAR = 0x1,
    D3D12_FILTER_MIN_POINT_MAG_LINEAR_MIP_POINT = 0x4,
    D3D12_FILTER_MIN_POINT_MAG_MIP_LINEAR = 0x5,
    D3D12_FILTER_MIN_LINEAR_MAG_MIP_POINT = 0x10,
    D3D12_FILTER_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x11,
    D3D12_FILTER_MIN_MAG_LINEAR_MIP_POINT = 0x14,
    D3D12_FILTER_MIN_MAG_MIP_LINEAR = 0x15,
    D3D12_FILTER_ANISOTROPIC = 0x55,
    D3D12_FILTER_COMPARISON_MIN_MAG_MIP_POINT = 0x80,
    D3D12_FILTER_COMPARISON_MIN_MAG_POINT_MIP_LINEAR = 0x81,
    D3D12_FILTER_COMPARISON_MIN_POINT_MAG_LINEAR_MIP_POINT = 0x84,
    D3D12_FILTER_COMPARISON_MIN_POINT_MAG_MIP_LINEAR = 0x85,
    D3D12_FILTER_COMPARISON_MIN_LINEAR_MAG_MIP_POINT = 0x90,
    D3D12_FILTER_COMPARISON_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x91,
    D3D12_FILTER_COMPARISON_MIN_MAG_LINEAR_MIP_POINT = 0x94,
    D3D12_FILTER_COMPARISON_MIN_MAG_MIP_LINEAR = 0x95,
    D3D12_FILTER_COMPARISON_ANISOTROPIC = 0xd5,
    D3D12_FILTER_MINIMUM_MIN_MAG_MIP_POINT = 0x100,
    D3D12_FILTER_MINIMUM_MIN_MAG_POINT_MIP_LINEAR = 0x101,
    D3D12_FILTER_MINIMUM_MIN_POINT_MAG_LINEAR_MIP_POINT = 0x104,
    D3D12_FILTER_MINIMUM_MIN_POINT_MAG_MIP_LINEAR = 0x105,
    D3D12_FILTER_MINIMUM_MIN_LINEAR_MAG_MIP_POINT = 0x110,
    D3D12_FILTER_MINIMUM_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x111,
    D3D12_FILTER_MINIMUM_MIN_MAG_LINEAR_MIP_POINT = 0x114,
    D3D12_FILTER_MINIMUM_MIN_MAG_MIP_LINEAR = 0x115,
    D3D12_FILTER_MINIMUM_ANISOTROPIC = 0x155,
    D3D12_FILTER_MAXIMUM_MIN_MAG_MIP_POINT = 0x180,
    D3D12_FILTER_MAXIMUM_MIN_MAG_POINT_MIP_LINEAR = 0x181,
    D3D12_FILTER_MAXIMUM_MIN_POINT_MAG_LINEAR_MIP_POINT = 0x184,
    D3D12_FILTER_MAXIMUM_MIN_POINT_MAG_MIP_LINEAR = 0x185,
    D3D12_FILTER_MAXIMUM_MIN_LINEAR_MAG_MIP_POINT = 0x190,
    D3D12_FILTER_MAXIMUM_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x191,
    D3D12_FILTER_MAXIMUM_MIN_MAG_LINEAR_MIP_POINT = 0x194,
    D3D12_FILTER_MAXIMUM_MIN_MAG_MIP_LINEAR = 0x195,
    D3D12_FILTER_MAXIMUM_ANISOTROPIC = 0x1d5
}