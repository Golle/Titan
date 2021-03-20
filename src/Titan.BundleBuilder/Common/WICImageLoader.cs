using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.WIC;
using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.D3D11.DXGI_FORMAT;
using static Titan.Windows.Win32.Native.CLSCTX;
using static Titan.Windows.Win32.Native.GENERIC_RIGHTS;
using static Titan.Windows.Win32.Native.Ole32;

namespace Titan.BundleBuilder.Common
{
    internal unsafe class ImageLoader : IDisposable
    {
        private ComPtr<IWICImagingFactory> _factory;

        public ImageLoader()
        {
            fixed (Guid* clsid = &WICCLSID.CLSID_WICImagingFactory2)
            {
                var riid = typeof(IWICImagingFactory).GUID;
                CheckAndThrow(CoCreateInstance(clsid, null, CLSCTX_INPROC_SERVER, &riid, (void**)_factory.GetAddressOf()), nameof(CoCreateInstance));
            }
        }

        public Image Load(string path)
        {
            using ComPtr<IWICBitmapDecoder> decoder = default;
            fixed (char* wzFilename = path)
            {
                CheckAndThrow(_factory.Get()->CreateDecoderFromFilename(wzFilename, null, (uint)GENERIC_READ, WICDecodeOptions.WICDecodeMetadataCacheOnDemand, decoder.GetAddressOf()), nameof(IWICImagingFactory.CreateDecoderFromFilename));
            }

            using ComPtr<IWICBitmapFrameDecode> frameDecode = default;
            CheckAndThrow(decoder.Get()->GetFrame(0, frameDecode.GetAddressOf()), nameof(IWICBitmapDecoder.GetFrame));

            Guid pixelFormat;
            CheckAndThrow(frameDecode.Get()->GetPixelFormat(&pixelFormat), nameof(IWICBitmapFrameDecode.GetPixelFormat));

            uint height, width;
            CheckAndThrow(frameDecode.Get()->GetSize(&width, &height), nameof(IWICBitmapFrameDecode.GetSize));

            var dxgiFormat = WICToDXGITranslationTable.Translate(pixelFormat);
            if (dxgiFormat == DXGI_FORMAT_UNKNOWN)
            {
                // needs conversion
                var newFormatGuid = WICConvertionTable.Convert(pixelFormat);
                var newBitsPerPixel = GetBitsPerPixel(newFormatGuid);

                var rowPitch = (width * newBitsPerPixel + 7) / 8;
                var imageSize = rowPitch * height;

                var buffer = new byte[imageSize];
                fixed(byte* pBuffer = buffer)
                {
                    using ComPtr<IWICFormatConverter> converter = default;
                    CheckAndThrow(_factory.Get()->CreateFormatConverter(converter.GetAddressOf()), nameof(IWICImagingFactory.CreateFormatConverter));
                    CheckAndThrow(converter.Get()->Initialize((IWICBitmapSource*)frameDecode.Get(), &newFormatGuid, WICBitmapDitherType.WICBitmapDitherTypeErrorDiffusion, null, 0, WICBitmapPaletteType.WICBitmapPaletteTypeCustom), nameof(IWICFormatConverter.Initialize));
                    CheckAndThrow(converter.Get()->CopyPixels(null, rowPitch, imageSize, pBuffer), nameof(IWICFormatConverter.CopyPixels));
                    return new Image(buffer, imageSize, WICToDXGITranslationTable.Translate(newFormatGuid), rowPitch, width, height);
                }
            }
            else
            {

                var bitsPerPixel = GetBitsPerPixel(pixelFormat);
                var stride = (width * bitsPerPixel + 7) / 8;
                var imageSize = stride * height;

                var buffer = new byte[imageSize];
                fixed(byte * pBuffer = buffer)
                {
                    CheckAndThrow(frameDecode.Get()->CopyPixels(null, stride, imageSize, pBuffer), nameof(IWICBitmapFrameDecode.CopyPixels));
                }
                return new Image(buffer, imageSize, dxgiFormat, stride, width, height);
            }
        }

        private uint GetBitsPerPixel(Guid guid)
        {
            using ComPtr<IWICComponentInfo> componentInfo = default;
            CheckAndThrow(_factory.Get()->CreateComponentInfo(&guid, componentInfo.GetAddressOf()), nameof(IWICImagingFactory.CreateComponentInfo));

            WICComponentType type;
            CheckAndThrow(componentInfo.Get()->GetComponentType(&type), nameof(IWICComponentInfo.GetComponentType));
            if (type != WICComponentType.WICPixelFormat)
            {
                throw new NotSupportedException($"Only {WICComponentType.WICPixelFormat} is suppported.");
            }

            var pixelFormatInfoGuid = typeof(IWICPixelFormatInfo).GUID;
            using ComPtr<IWICPixelFormatInfo> info = default;
            CheckAndThrow(componentInfo.Get()->QueryInterface(&pixelFormatInfoGuid, (void**)info.GetAddressOf()), nameof(IWICComponentInfo.QueryInterface));

            uint bitsPerPixel;
            CheckAndThrow(info.Get()->GetBitsPerPixel(&bitsPerPixel), nameof(IWICPixelFormatInfo.GetBitsPerPixel));
            return bitsPerPixel;
        }

        public void Dispose() => _factory.Dispose();
    }
}
