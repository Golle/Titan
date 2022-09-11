using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.Win32;


[StructLayout(LayoutKind.Sequential)]
public unsafe struct HIDP_VALUE_CAPS
{
    public HID_USAGE_PAGE UsagePage;
    public byte ReportID;
    public byte IsAlias;
    public ushort BitField;
    public ushort LinkCollection;   // A unique internal index pointer
    public ushort /*USAGE*/ LinkUsage;
    public ushort /*USAGE*/ LinkUsagePage;
    public byte IsRange;
    public byte IsStringRange;
    public byte IsDesignatorRange;
    public byte IsAbsolute;
    public byte HasNull;        // Does this channel have a null report   union
    public byte Reserved;
    public ushort BitSize;        // How many bits are devoted to this value?
    public ushort ReportCount;    // See Note below.  Usually set to 1.
    public fixed ushort Reserved2[5];
    public uint UnitsExp;
    public uint Units;
    public int LogicalMin, LogicalMax;
    public int PhysicalMin, PhysicalMax;

    private ulong _rangeNotRange0;
    private ulong _rangeNotRange1;
    public ref HidPRange Range => ref *(HidPRange*)Unsafe.AsPointer(ref _rangeNotRange0);
    public ref HidPNotRange NotRange => ref *(HidPNotRange*)Unsafe.AsPointer(ref _rangeNotRange0);
}
