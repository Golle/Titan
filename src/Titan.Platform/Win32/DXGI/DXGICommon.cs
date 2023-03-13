using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.DXGI;

public static unsafe partial class DXGICommon
{
    private const string DllName = "dxgi";
    
    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial HRESULT CreateDXGIFactory1(Guid* riid, void** ppFactory);

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial HRESULT CreateDXGIFactory2(
        DXGI_CREATE_FACTORY_FLAGS Flags,
        Guid* riid,
        void** ppFactory
    );
}
