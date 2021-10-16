using System;
using System.Runtime.InteropServices;
using Titan.Windows.D3D;
// ReSharper disable InconsistentNaming

namespace Titan.Windows.D3D12;

public static unsafe class D3D12Common
{

    [DllImport("d3d12", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT D3D12CreateDevice(
        IUnknown* pAdapter,
        D3D_FEATURE_LEVEL minimumFeatureLevel,
        in Guid riid, // Expected: ID3D12Device
        void** ppDevice);
}
