
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.WIC;

public unsafe struct WICCLSID
{
    public static Guid* CLSID_WICImagingFactory => AsPointer(new byte[] { 0x62, 0xf2, 0xca, 0xca, 0x70, 0x93, 0x15, 0x46, 0xa1, 0x3b, 0x9f, 0x55, 0x39, 0xda, 0x4c, 0x0a });
    public static Guid* CLSID_WICImagingFactory1 => AsPointer(new byte[] { 0x62, 0xf2, 0xca, 0xca, 0x70, 0x93, 0x15, 0x46, 0xa1, 0x3b, 0x9f, 0x55, 0x39, 0xda, 0x4c, 0x0a });
    public static Guid* CLSID_WICImagingFactory2 => AsPointer(new byte[] { 0xe8, 0x06, 0x7d, 0x31, 0x24, 0x5f, 0x3d, 0x43, 0xbd, 0xf7, 0x79, 0xce, 0x68, 0xd8, 0xab, 0xc2 });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Guid* AsPointer(in ReadOnlySpan<byte> data)
    {
        Debug.Assert(data.Length == sizeof(Guid));
        return (Guid*)Unsafe.AsPointer(ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data)));
    }
}
