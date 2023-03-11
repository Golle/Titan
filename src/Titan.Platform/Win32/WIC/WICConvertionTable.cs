using static Titan.Platform.Win32.WIC.WICPixelFormats;
namespace Titan.Platform.Win32.WIC;

public class WICConvertionTable
{
    private static readonly IDictionary<Guid, Guid> _convertionTable = new Dictionary<Guid, Guid>()
    {
        {GUID_WICPixelFormatBlackWhite, GUID_WICPixelFormat8bppGray}, // DXGI_FORMAT_R8_UNORM

        {GUID_WICPixelFormat1bppIndexed, GUID_WICPixelFormat32bppRGBA}, // DXGI_FORMAT_R8G8B8A8_UNORM 
        {GUID_WICPixelFormat2bppIndexed, GUID_WICPixelFormat32bppRGBA}, // DXGI_FORMAT_R8G8B8A8_UNORM 
        {GUID_WICPixelFormat4bppIndexed, GUID_WICPixelFormat32bppRGBA}, // DXGI_FORMAT_R8G8B8A8_UNORM 
        {GUID_WICPixelFormat8bppIndexed, GUID_WICPixelFormat32bppRGBA}, // DXGI_FORMAT_R8G8B8A8_UNORM 

        {GUID_WICPixelFormat2bppGray, GUID_WICPixelFormat8bppGray}, // DXGI_FORMAT_R8_UNORM 
        {GUID_WICPixelFormat4bppGray, GUID_WICPixelFormat8bppGray}, // DXGI_FORMAT_R8_UNORM 

        {GUID_WICPixelFormat16bppGrayFixedPoint, GUID_WICPixelFormat16bppGrayHalf}, // DXGI_FORMAT_R16_FLOAT 
        {GUID_WICPixelFormat32bppGrayFixedPoint, GUID_WICPixelFormat32bppGrayFloat}, // DXGI_FORMAT_R32_FLOAT 

//#ifdef DXGI_1_2_FORMATSs
        //{GUID_WICPixelFormat16bppBGR555, GUID_WICPixelFormat16bppBGRA5551}, // DXGI_FORMAT_B5G5R5A1_UNORM

//#else
        {GUID_WICPixelFormat16bppBGR555, GUID_WICPixelFormat32bppRGBA}, // DXGI_FORMAT_R8G8B8A8_UNORM
        {GUID_WICPixelFormat16bppBGRA5551, GUID_WICPixelFormat32bppRGBA}, // DXGI_FORMAT_R8G8B8A8_UNORM
        {GUID_WICPixelFormat16bppBGR565, GUID_WICPixelFormat32bppRGBA}, // DXGI_FORMAT_R8G8B8A8_UNORM

//#endif // DXGI_1_2_FORMATS

        {GUID_WICPixelFormat32bppBGR101010, GUID_WICPixelFormat32bppRGBA1010102}, // DXGI_FORMAT_R10G10B10A2_UNORM

        {GUID_WICPixelFormat24bppBGR, GUID_WICPixelFormat32bppRGBA}, // DXGI_FORMAT_R8G8B8A8_UNORM 
        {GUID_WICPixelFormat24bppRGB, GUID_WICPixelFormat32bppRGBA}, // DXGI_FORMAT_R8G8B8A8_UNORM 
        {GUID_WICPixelFormat32bppPBGRA, GUID_WICPixelFormat32bppRGBA}, // DXGI_FORMAT_R8G8B8A8_UNORM 
        {GUID_WICPixelFormat32bppPRGBA, GUID_WICPixelFormat32bppRGBA}, // DXGI_FORMAT_R8G8B8A8_UNORM 

        {GUID_WICPixelFormat48bppRGB, GUID_WICPixelFormat64bppRGBA}, // DXGI_FORMAT_R16G16B16A16_UNORM
        {GUID_WICPixelFormat48bppBGR, GUID_WICPixelFormat64bppRGBA}, // DXGI_FORMAT_R16G16B16A16_UNORM
        {GUID_WICPixelFormat64bppBGRA, GUID_WICPixelFormat64bppRGBA}, // DXGI_FORMAT_R16G16B16A16_UNORM
        {GUID_WICPixelFormat64bppPRGBA, GUID_WICPixelFormat64bppRGBA}, // DXGI_FORMAT_R16G16B16A16_UNORM
        {GUID_WICPixelFormat64bppPBGRA, GUID_WICPixelFormat64bppRGBA}, // DXGI_FORMAT_R16G16B16A16_UNORM

        {GUID_WICPixelFormat48bppRGBFixedPoint, GUID_WICPixelFormat64bppRGBAHalf}, // DXGI_FORMAT_R16G16B16A16_FLOAT 
        {GUID_WICPixelFormat48bppBGRFixedPoint, GUID_WICPixelFormat64bppRGBAHalf}, // DXGI_FORMAT_R16G16B16A16_FLOAT 
        {GUID_WICPixelFormat64bppRGBAFixedPoint, GUID_WICPixelFormat64bppRGBAHalf}, // DXGI_FORMAT_R16G16B16A16_FLOAT 
        {GUID_WICPixelFormat64bppBGRAFixedPoint, GUID_WICPixelFormat64bppRGBAHalf}, // DXGI_FORMAT_R16G16B16A16_FLOAT 
        {GUID_WICPixelFormat64bppRGBFixedPoint, GUID_WICPixelFormat64bppRGBAHalf}, // DXGI_FORMAT_R16G16B16A16_FLOAT 
        {GUID_WICPixelFormat64bppRGBHalf, GUID_WICPixelFormat64bppRGBAHalf}, // DXGI_FORMAT_R16G16B16A16_FLOAT 
        {GUID_WICPixelFormat48bppRGBHalf, GUID_WICPixelFormat64bppRGBAHalf}, // DXGI_FORMAT_R16G16B16A16_FLOAT 

        {GUID_WICPixelFormat96bppRGBFixedPoint, GUID_WICPixelFormat128bppRGBAFloat}, // DXGI_FORMAT_R32G32B32A32_FLOAT 
        {GUID_WICPixelFormat128bppPRGBAFloat, GUID_WICPixelFormat128bppRGBAFloat}, // DXGI_FORMAT_R32G32B32A32_FLOAT 
        {GUID_WICPixelFormat128bppRGBFloat, GUID_WICPixelFormat128bppRGBAFloat}, // DXGI_FORMAT_R32G32B32A32_FLOAT 
        {GUID_WICPixelFormat128bppRGBAFixedPoint, GUID_WICPixelFormat128bppRGBAFloat}, // DXGI_FORMAT_R32G32B32A32_FLOAT 
        {GUID_WICPixelFormat128bppRGBFixedPoint, GUID_WICPixelFormat128bppRGBAFloat}, // DXGI_FORMAT_R32G32B32A32_FLOAT 

        {GUID_WICPixelFormat32bppCMYK, GUID_WICPixelFormat32bppRGBA}, // DXGI_FORMAT_R8G8B8A8_UNORM 
        {GUID_WICPixelFormat64bppCMYK, GUID_WICPixelFormat64bppRGBA}, // DXGI_FORMAT_R16G16B16A16_UNORM
        {GUID_WICPixelFormat40bppCMYKAlpha, GUID_WICPixelFormat64bppRGBA}, // DXGI_FORMAT_R16G16B16A16_UNORM
        {GUID_WICPixelFormat80bppCMYKAlpha, GUID_WICPixelFormat64bppRGBA}, // DXGI_FORMAT_R16G16B16A16_UNORM

//#if (_WIN32_WINNT >= 0x0602 /*_WIN32_WINNT_WIN8*/)
        {GUID_WICPixelFormat32bppRGB, GUID_WICPixelFormat32bppRGBA}, // DXGI_FORMAT_R8G8B8A8_UNORM
        {GUID_WICPixelFormat64bppRGB, GUID_WICPixelFormat64bppRGBA}, // DXGI_FORMAT_R16G16B16A16_UNORM
        {GUID_WICPixelFormat64bppPRGBAHalf, GUID_WICPixelFormat64bppRGBAHalf}, // DXGI_FORMAT_R16G16B16A16_FLOAT
    };
//#endif
    public static Guid Convert(Guid WICGuid) => _convertionTable.TryGetValue(WICGuid, out var convertedValue) ? convertedValue : throw new NotSupportedException($"Format {WICGuid} is not supported for convertion.");
}
