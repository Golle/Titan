using System.Runtime.InteropServices;
using Titan.Platform.Win32;

namespace Titan.Platform.DXC;

public static unsafe class DXCCompilerCommon
{
    //NOTE(Jens): currently using a hardcoded path, will change when we have something that downloads the latest version
    private const string DllName = "dxcompiler";

    public static readonly Guid CLSID_Compiler = new(0x73e22d93, 0xe6ce, 0x47f3, 0xb5, 0xbf, 0xf0, 0x66, 0x4f, 0x39, 0xc1, 0xb0);
    public static readonly Guid CLSID_DxcLibrary = new(0x6245d6af, 0x66e0, 0x48fd, 0x80, 0xb4, 0x4d, 0x27, 0x17, 0x96, 0x74, 0x8c);
    public static readonly Guid CLSID_DxcUtils = CLSID_DxcLibrary;

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    public static extern HRESULT DxcCreateInstance(
        in Guid rclsid,
        in Guid riid,
        void** ppv
    );

}
