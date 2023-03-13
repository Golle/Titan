using System.Runtime.InteropServices;

namespace Titan.Platform.Win32;

public static unsafe class Ole32
{
    [DllImport("Ole32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT CoCreateInstance(
        Guid* rclsid,
        void* pUnkOuter,
        CLSCTX dwClsContext,
        Guid* riid,
        void** ppv
    );
}
