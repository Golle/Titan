using System;
using System.Runtime.InteropServices;

namespace Titan.Windows.Win32
{
    public static unsafe class Ole32
    {
        [DllImport("Ole32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern HRESULT CoCreateInstance(
            [In] Guid* rclsid,
            [In] void* pUnkOuter,
            [In] CLSCTX dwClsContext,
            [In] Guid* riid,
            [Out] void** ppv
        );
    }
}
