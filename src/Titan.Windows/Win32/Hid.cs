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

    [DllImport(HidDll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern NTSTATUS HidP_GetButtonCaps(
        HIDP_REPORT_TYPE ReportType,
        HIDP_BUTTON_CAPS* ButtonCaps,
        ushort* ButtonCapsLength,
        void* /*HIDP_PREPARSED_DATA*/ PreparsedData
    );


    [DllImport(HidDll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern NTSTATUS HidP_GetValueCaps(
        HIDP_REPORT_TYPE ReportType,
        HIDP_VALUE_CAPS* ValueCaps,
        ushort* ValueCapsLength,
        void* /*HIDP_PREPARSED_DATA*/ PreparsedData
    );

    [DllImport(HidDll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern NTSTATUS HidP_GetUsages(
        HIDP_REPORT_TYPE ReportType,
        HID_USAGE_PAGE UsagePage,
        ushort LinkCollection,
        ushort* /*PUSAGE*/ UsageList,
        uint* UsageLength,
        void* /*PHIDP_PREPARSED_DATA */PreparsedData,
        byte* Report,
        uint ReportLength
    );

    [DllImport(HidDll, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern NTSTATUS HidP_GetUsageValue(
        HIDP_REPORT_TYPE ReportType,
        HID_USAGE_PAGE UsagePage,
        ushort LinkCollection,
        ushort Usage,
        uint* UsageValue,
        void* /*PHIDP_PREPARSED_DATA*/ PreparsedData,
        byte* Report,
        uint ReportLength
    );
}
