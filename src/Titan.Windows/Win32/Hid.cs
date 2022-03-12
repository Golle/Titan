using System.Runtime.InteropServices;

namespace Titan.Windows.Win32;

public static unsafe class Hid
{
    private const string HidDll = "hid";

    [DllImport(HidDll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern NTSTATUS HidP_GetCaps(
        void* PreparsedData,
        HIDP_CAPS* Capabilities
    );
}
