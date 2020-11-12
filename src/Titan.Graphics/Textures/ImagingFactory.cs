using System;
using Titan.Windows.Win32;
using Titan.Windows.Win32.WIC;

using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.Native.CLSCTX;
using static Titan.Windows.Win32.Native.GENERIC_RIGHTS;
using static Titan.Windows.Win32.Native.Ole32;
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

        public IImageDecoder CreateDecoderFromFilename(string filename)
        {
            using ComPtr<IWICBitmapDecoder> decoder = default;
            fixed (char* wzFilename = filename)
            {
                CheckAndThrow(_factory.Get()->CreateDecoderFromFilename(wzFilename, null, (uint) GENERIC_READ, WICDecodeMetadataCacheOnDemand, decoder.GetAddressOf()), "CreateDecoderFromFilename");
            }

            using ComPtr<IWICBitmapFrameDecode> frameDecode = default;
            CheckAndThrow(decoder.Get()->GetFrame(0, frameDecode.GetAddressOf()), "GetFrame");
            
            Guid pixelFormat;
            CheckAndThrow(frameDecode.Get()->GetPixelFormat(&pixelFormat), "GetPixelFormat");



            //using ComPtr<IWICComponentInfo> componentInfo = default;
            //CheckAndThrow(_factory.Get()->CreateComponentInfo(&pixelFormat, componentInfo.GetAddressOf()), "CreateComponentInfo");

            //WICComponentType componentType;
            //CheckAndThrow(componentInfo.Get()->GetComponentType(&componentType), "GetComponentType");
            //if (componentType != WICComponentType.WICPixelFormat)
            //{
            //    throw new NotSupportedException($"Component type not supported. {componentType}");
            //}




            //var pixelFormatInfoGuid = typeof(IWICPixelFormatInfo).GUID;
            //using ComPtr<IWICPixelFormatInfo> pixelFormatInfo = default;
            //CheckAndThrow(componentInfo.Get()->QueryInterface(&pixelFormatInfoGuid, (void**) pixelFormatInfo.GetAddressOf()), "QueryInterface");

            //uint bitsPerPixel;
            //CheckAndThrow(pixelFormatInfo.Get()->GetBitsPerPixel(&bitsPerPixel), "GetBitsPerPixel");

            var bitsPerPixel = GetBitsPerPixel(pixelFormat);

            uint height, width;
            CheckAndThrow(frameDecode.Get()->GetSize(&width, &height), "GetSize");
            

            return new ImageDecoder(frameDecode, width, height, pixelFormat, bitsPerPixel);
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
