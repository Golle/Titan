namespace Titan.Platform.Win32;

public unsafe struct SYSTEM_INFO
{
    //union {
    //    DWORD dwOemId;          // Obsolete field...do not use
    //    struct {
    //        WORD wProcessorArchitecture;
    //        WORD wReserved;
    //    }
    //    DUMMYSTRUCTNAME;
    //} DUMMYUNIONNAME;
    public uint dwOemId;
    public uint dwPageSize;
    public void* lpMinimumApplicationAddress;
    public void* lpMaximumApplicationAddress;
    public uint* dwActiveProcessorMask;
    public uint dwNumberOfProcessors;
    public uint dwProcessorType;
    public uint dwAllocationGranularity;
    public ushort wProcessorLevel;
    public ushort wProcessorRevision;
}
