using System;
using System.Runtime.InteropServices;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using Titan.Windows.Win32.WIC;

using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.Native.CLSCTX;
using static Titan.Windows.Win32.Native.GENERIC_RIGHTS;
using static Titan.Windows.Win32.Native.Ole32;
using static Titan.Windows.Win32.WIC.WICBitmapDitherType;
using static Titan.Windows.Win32.WIC.WICBitmapPaletteType;
using static Titan.Windows.Win32.WIC.WICDecodeOptions;

namespace Titan.Graphics.Textures
{
    internal unsafe class ImagingFactory : IImagingFactory
    {
        private ComPtr<IWICImagingFactory> _factory;

        public ImagingFactory()
        {
            fixed (Guid* clsid = &WICCLSID.CLSID_WICImagingFactory2)
            {
                var riid = typeof(IWICImagingFactory).GUID;
                CheckAndThrow(CoCreateInstance(clsid, null, CLSCTX_INPROC_SERVER, &riid, (void**)_factory.GetAddressOf()), "CoCreateInstance");
            }
        }
        public IImage LoadImageFromFile(string filename)
        {
            using ComPtr<IWICBitmapDecoder> decoder = default;
            fixed (char* wzFilename = filename)
            {
                CheckAndThrow(_factory.Get()->CreateDecoderFromFilename(wzFilename, null, (uint)GENERIC_READ, WICDecodeMetadataCacheOnDemand, decoder.GetAddressOf()), "CreateDecoderFromFilename");
            }

            using ComPtr<IWICBitmapFrameDecode> frameDecode = default;
            CheckAndThrow(decoder.Get()->GetFrame(0, frameDecode.GetAddressOf()), "GetFrame");

            Guid pixelFormat;
            CheckAndThrow(frameDecode.Get()->GetPixelFormat(&pixelFormat), "GetPixelFormat");

            uint height, width;
            CheckAndThrow(frameDecode.Get()->GetSize(&width, &height), "GetSize");

            var dxgiFormat = WICToDXGITranslationTable.Translate(pixelFormat);
            if (dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_UNKNOWN)
            {
                // needs conversion
                var newFormatGuid = WICConvertionTable.Convert(pixelFormat);
                var newBitsPerPixel = GetBitsPerPixel(newFormatGuid);
                
                var rowPitch = (width* newBitsPerPixel + 7) / 8;
                var imageSize = rowPitch * height;

                var buffer = (byte*)Marshal.AllocHGlobal((int)imageSize);
                try
                {
                    using ComPtr<IWICFormatConverter> converter = default;
                    CheckAndThrow(_factory.Get()->CreateFormatConverter(converter.GetAddressOf()), "CreateFormatConverter");
                    CheckAndThrow(converter.Get()->Initialize((IWICBitmapSource*)frameDecode.Get(), &newFormatGuid, WICBitmapDitherTypeErrorDiffusion, null, 0, WICBitmapPaletteTypeCustom), "Initialize");
                    CheckAndThrow(converter.Get()->CopyPixels(null, rowPitch, imageSize, buffer), "CopyPixels");

                    return new Image(buffer, imageSize, WICToDXGITranslationTable.Translate(newFormatGuid), rowPitch, width, height);
                }
                catch
                {
                    // If there's an exception, free the buffer
                    Marshal.FreeHGlobal((nint)buffer);
                    throw;
                }
            }
            else
            {

                var bitsPerPixel = GetBitsPerPixel(pixelFormat);
                var stride = (width * bitsPerPixel + 7) / 8;
                var imageSize = stride * height;

                var buffer = (byte*)Marshal.AllocHGlobal((int)imageSize);
                try
                {
                    CheckAndThrow(frameDecode.Get()->CopyPixels(null, stride, imageSize, buffer), "CopyPixels");
                }
                catch
                {
                    // If there's an exception, free the buffer
                    Marshal.FreeHGlobal((nint)buffer);
                    throw;
                }
                
                return new Image(buffer, imageSize, dxgiFormat, stride,  width, height);
            }
        }

        private uint GetBitsPerPixel(Guid guid)
        {
            using ComPtr<IWICComponentInfo> componentInfo = default;
            CheckAndThrow(_factory.Get()->CreateComponentInfo(&guid, componentInfo.GetAddressOf()), "CreateComponentInfo");

            WICComponentType type;
            CheckAndThrow(componentInfo.Get()->GetComponentType(&type), "GetComponentType");
            if (type != WICComponentType.WICPixelFormat)
            {
                throw new NotSupportedException($"Only {WICComponentType.WICPixelFormat} is suppported.");
            }

            var pixelFormatInfoGuid = typeof(IWICPixelFormatInfo).GUID;
            using ComPtr<IWICPixelFormatInfo> info = default;
            CheckAndThrow(componentInfo.Get()->QueryInterface(&pixelFormatInfoGuid, (void**) info.GetAddressOf()), "QueryInterface");

            uint bitsPerPixel;
            CheckAndThrow(info.Get()->GetBitsPerPixel(&bitsPerPixel), "GetBitsPerPixel");
            return bitsPerPixel;
        }


        public void Dispose() => _factory.Dispose();
    }
}
