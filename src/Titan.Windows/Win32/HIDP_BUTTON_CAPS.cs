using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Windows.Win32;

[SkipLocalsInit]
[StructLayout(LayoutKind.Sequential)]
public unsafe struct HIDP_BUTTON_CAPS
{
    public HID_USAGE_PAGE UsagePage;
    public byte ReportID;
    public byte IsAlias;
    public ushort BitField;
    public ushort LinkCollection;   // A unique internal index pointer
    public ushort /*USAGE*/ LinkUsage;
    public ushort /*USAGE */ LinkUsagePage;
    public byte IsRange;
    public byte IsStringRange;
    public byte IsDesignatorRange;
    public byte IsAbsolute;
    public fixed uint Reserved[10];

    private ulong _rangeNotRange0;
    private ulong _rangeNotRange1;

    public ref HidPRange Range => ref *(HidPRange*)Unsafe.AsPointer(ref _rangeNotRange0);
    public ref HidPNotRange NotRange => ref *(HidPNotRange*)Unsafe.AsPointer(ref _rangeNotRange0);
    
}
