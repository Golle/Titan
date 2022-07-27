using System;
using System.Runtime.InteropServices;
using Titan.Windows.D3D;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.D3D12;

public static unsafe class D3D12Common
{

    private const string DllName = "d3d12";
    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT D3D12CreateDevice(
        IUnknown* pAdapter,
        D3D_FEATURE_LEVEL minimumFeatureLevel,
        in Guid riid, // Expected: ID3D12Device
        void** ppDevice);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT D3D12GetDebugInterface(
        in Guid riid,
        void** ppvDebug
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT D3D12SerializeRootSignature(
        D3D12_ROOT_SIGNATURE_DESC* pRootSignature,
        D3D_ROOT_SIGNATURE_VERSION Version,
        ID3DBlob** ppBlob,
        ID3DBlob** ppErrorBlob
    );
}
