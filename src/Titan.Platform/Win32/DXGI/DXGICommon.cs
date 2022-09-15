using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.DXGI;

public static unsafe class DXGICommon
{
    private const string DllName = "dxgi";
    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT CreateDXGIFactory1(in Guid riid, void** ppFactory);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT CreateDXGIFactory2(
        DXGI_CREATE_FACTORY_FLAGS Flags,
        in Guid riid,
        void** ppFactory
    );
}
