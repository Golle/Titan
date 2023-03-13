using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32;

namespace Titan.Platform.DXC;

public static unsafe class DXCCompilerCommon
{
    //NOTE(Jens): currently using a hardcoded path, will change when we have something that downloads the latest version
    private const string DllName = "dxcompiler";

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern HRESULT DxcCreateInstance(
        Guid* rclsid,
        Guid* riid,
        void** ppv
    );

    public static Guid* CLSID_Compiler => AsPointer(new byte[] { 0x93, 0x2d, 0xe2, 0x73, 0xce, 0xe6, 0xf3, 0x47, 0xb5, 0xbf, 0xf0, 0x66, 0x4f, 0x39, 0xc1, 0xb0 });
    public static Guid* CLSID_DxcLibrary => AsPointer(new byte[] { 0xaf, 0xd6, 0x45, 0x62, 0xe0, 0x66, 0xfd, 0x48, 0x80, 0xb4, 0x4d, 0x27, 0x17, 0x96, 0x74, 0x8c });
    public static Guid* CLSID_DxcUtils => CLSID_DxcLibrary;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Guid* AsPointer(in ReadOnlySpan<byte> data)
    {
        Debug.Assert(data.Length == sizeof(Guid));
        return (Guid*)Unsafe.AsPointer(ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data)));
    }

}
