namespace Titan.Platform.Win32.WIC;

public enum WICComponentType : uint
{
    WICDecoder = 0x1,
    WICEncoder = 0x2,
    WICPixelFormatConverter = 0x4,
    WICMetadataReader = 0x8,
    WICMetadataWriter = 0x10,
    WICPixelFormat = 0x20,
    WICAllComponents = 0x3f,
    WICCOMPONENTTYPE_FORCE_DWORD = 0x7fffffff
}
