using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.D3D;

namespace Titan.Platform.Win32.D3D12;

public unsafe partial struct D3D12Common
{
    public const int D3D12_MAX_SHADER_VISIBLE_DESCRIPTOR_HEAP_SIZE_TIER_1 = 1000000;
    public const int D3D12_MAX_SHADER_VISIBLE_DESCRIPTOR_HEAP_SIZE_TIER_2 = 1000000;
    public const int D3D12_MAX_SHADER_VISIBLE_SAMPLER_HEAP_SIZE = 2048;

    private const string DllName = "d3d12";

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial HRESULT D3D12CreateDevice(
        IUnknown* pAdapter,
        D3D_FEATURE_LEVEL minimumFeatureLevel,
        Guid* riid, // Expected: ID3D12Device
        void** ppDevice);

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial HRESULT D3D12GetDebugInterface(
        Guid* riid,
        void** ppvDebug
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial HRESULT D3D12SerializeRootSignature(
        D3D12_ROOT_SIGNATURE_DESC* pRootSignature,
        D3D_ROOT_SIGNATURE_VERSION Version,
        ID3DBlob** ppBlob,
        ID3DBlob** ppErrorBlob
    );

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial HRESULT D3D12SerializeVersionedRootSignature(
        D3D12_VERSIONED_ROOT_SIGNATURE_DESC* pRootSignature,
        ID3DBlob** ppBlob,
        ID3DBlob** ppErrorBlob
    );
}
