using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.Win32;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct HIDP_CAPS
{
    public HID_USAGE Usage;
    public HID_USAGE_PAGE UsagePage;
    public ushort InputReportByteLength;
    public ushort OutputReportByteLength;
    public ushort FeatureReportByteLength;
    public fixed ushort Reserved[17];
    public ushort NumberLinkCollectionNodes;
    public ushort NumberInputButtonCaps;
    public ushort NumberInputValueCaps;
    public ushort NumberInputDataIndices;
    public ushort NumberOutputButtonCaps;
    public ushort NumberOutputValueCaps;
    public ushort NumberOutputDataIndices;
    public ushort NumberFeatureButtonCaps;
    public ushort NumberFeatureValueCaps;
    public ushort NumberFeatureDataIndices;
}
