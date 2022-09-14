using Titan.Core.Logging;
using Titan.Platform.Win32;
using Titan.Platform.Win32.DXGI;
using Titan.Platform.Win32.WIC;
using Titan.Platform.Win32.Win32;
using static Titan.Platform.Win32.Common;
using static Titan.Platform.Win32.Win32.Ole32;

namespace Titan.Tools.Core.Images;

public unsafe class ImageReader : IDisposable
{
    private ComPtr<IWICImagingFactory> _factory;
    public ImageReader()
    {
        var hr = CoCreateInstance(WICCLSID.CLSID_WICImagingFactory2, null, CLSCTX.CLSCTX_INPROC_SERVER, typeof(IWICImagingFactory).GUID, (void**)_factory.GetAddressOf());
        if (FAILED(hr))
        {
            throw new Exception($"Failed to create the {nameof(IWICImagingFactory)} instance with HRESULT {hr}");
        }
    }

    public Image? LoadImage(string path)
    {
        using ComPtr<IWICBitmapDecoder> decoder = default;
        HRESULT hr;
        fixed (char* pPath = path)
        {
            hr = _factory.Get()->CreateDecoderFromFilename(pPath, null, (uint)GENERIC_RIGHTS.GENERIC_READ, WICDecodeOptions.WICDecodeMetadataCacheOnDemand, decoder.GetAddressOf());
            if (FAILED(hr))
            {
                Logger.Error<ImageReader>($"Failed to create the {nameof(IWICBitmapDecoder)} with HRESULT {hr}");
                return null;
            }
        }

        using ComPtr<IWICBitmapFrameDecode> frameDecode = default;
        hr = decoder.Get()->GetFrame(0, frameDecode.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<ImageReader>($"Failed to get the frame with HRESULT {hr}");
            return null;
        }

        Guid pixelFormat;
        hr = frameDecode.Get()->GetPixelFormat(&pixelFormat);
        if (FAILED(hr))
        {
            Logger.Error<ImageReader>($"Failed to get the PixelFormat with HRESULT {hr}");
            return null;
        }

        uint height, width;
        hr = frameDecode.Get()->GetSize(&width, &height);
        if (FAILED(hr))
        {
            Logger.Error<ImageReader>($"Failed to get the image size with HRESULT {hr}");
            return null;
        }

        var dxgiFormat = WICToDXGITranslationTable.Translate(pixelFormat);
        if (dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_UNKNOWN)
        {
            Logger.Error<ImageReader>($"Can't load the pixel, a conversion is required. This is not implemented yet.");
            return null;
        }
        Logger.Trace<ImageReader>($"Height: {height} Width: {width} DXGI_FORMAT: {dxgiFormat}");


        if (!GetBitsPerPixel(pixelFormat, out var bitsPerPixel))
        {
            Logger.Error<ImageReader>("Failed to get the BitsPerPixel.");
            return null;
        }

        var stride = (width * bitsPerPixel + 7) / 8;
        var imageSize = stride * height;

        Logger.Trace<ImageReader>($"Size: {imageSize} Stride: {stride} BitsPerPixel: {bitsPerPixel}");
        var buffer = new byte[imageSize];
        fixed (byte* pBuffer = buffer)
        {
            hr = frameDecode.Get()->CopyPixels(null, stride, imageSize, pBuffer);
            if (FAILED(hr))
            {
                Logger.Error<ImageReader>($"Failed to CopyPixels with HRESULT {hr}");
                return null;
            }
        }
        return new Image
        {
            Height = height,
            Width = width,
            BitsPerPixel = bitsPerPixel,
            Data = buffer,
            Format = dxgiFormat,
            Stride = stride
        };
    }
    private bool GetBitsPerPixel(Guid guid, out uint bitsPerPixel)
    {
        bitsPerPixel = default;

        using ComPtr<IWICComponentInfo> componentInfo = default;
        var hr = _factory.Get()->CreateComponentInfo(&guid, componentInfo.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<ImageReader>($"Failed to CreateComponentInfo with HRESULT {hr}");
            return false;
        }
        WICComponentType type;
        hr = componentInfo.Get()->GetComponentType(&type);
        if (FAILED(hr))
        {
            Logger.Error<ImageReader>($"Failed to GetComponentType with HRESULT {hr}");
            return false;
        }
        if (type != WICComponentType.WICPixelFormat)
        {
            Logger.Error<ImageReader>($"Only {nameof(WICComponentType.WICPixelFormat)} is supported. Got {type}.");
            return false;
        }

        var pixelFormatInfoGuid = typeof(IWICPixelFormatInfo).GUID;
        using ComPtr<IWICPixelFormatInfo> info = default;
        hr = componentInfo.Get()->QueryInterface(&pixelFormatInfoGuid, (void**)info.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<ImageReader>($"Failed to QueryInterface for {nameof(IWICPixelFormatInfo)} with HRESULT {hr}");
            return false;
        }

        fixed (uint* pBitsPerPixel = &bitsPerPixel)
        {
            hr = info.Get()->GetBitsPerPixel(pBitsPerPixel);
            if (FAILED(hr))
            {
                Logger.Error<ImageReader>($"Failed to GetBitsPerPixel with HRESULT {hr}");
                return false;
            }
        }

        return true;
    }

    public void Dispose()
    {
        _factory.Dispose();
    }
}


public record Image
{
    public required byte[] Data { get; init; }
    public required uint BitsPerPixel { get; init; }
    public required uint Width { get; init; }
    public required uint Height { get; init; }
    public required DXGI_FORMAT Format { get; init; }
    public required uint Stride { get; init; }
    public int Size => Data.Length;
}
