using System.Runtime.InteropServices;

namespace Titan.Platform.Win32;

public static unsafe class Ole32
{
    [DllImport("Ole32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT CoCreateInstance(
        in Guid rclsid,
        void* pUnkOuter,
        CLSCTX dwClsContext,
        in Guid riid,
         void** ppv
    );
}
