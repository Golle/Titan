using System;
using System.Runtime.InteropServices;
using Titan.Core.IO;
using Titan.Windows;
using Titan.Windows.DXGI;
using Titan.Windows.WIC;
using static Titan.Windows.Common;
using static Titan.Windows.Win32.CLSCTX;
using static Titan.Windows.Win32.GENERIC_RIGHTS;
using static Titan.Windows.Win32.Ole32;
using static Titan.Windows.WIC.WICBitmapDitherType;
using static Titan.Windows.WIC.WICBitmapPaletteType;
using static Titan.Windows.WIC.WICDecodeOptions;

// ReSharper disable InconsistentNaming

namespace Titan.Graphics.Images
{
    public unsafe class WICImageLoader : IImageLoader
    {
        private ComPtr<IWICImagingFactory> _factory;

        public WICImageLoader()
        {
            CheckAndThrow(CoCreateInstance(WICCLSID.CLSID_WICImagingFactory2, null, CLSCTX_INPROC_SERVER, typeof(IWICImagingFactory).GUID, (void**)_factory.GetAddressOf()), nameof(CoCreateInstance));
        }

        public Image Load(byte* buffer, uint size)
        {
            using ComPtr<IWICStream> stream = default;
            CheckAndThrow(_factory.Get()->CreateStream(stream.GetAddressOf()), nameof(IWICImagingFactory.CreateStream));
            CheckAndThrow(stream.Get()->InitializeFromMemory(buffer, size), nameof(IWICStream.InitializeFromMemory));

            using ComPtr<IWICBitmapDecoder> decoder = default;
            CheckAndThrow(_factory.Get()->CreateDecoderFromStream((IStream*)stream.Get(), null, WICDecodeMetadataCacheOnDemand, decoder.GetAddressOf()), nameof(IWICImagingFactory.CreateDecoderFromStream));
            return LoadInternal(decoder);
        }

        public Image Load(ReadOnlySpan<byte> buffer)
        {
            fixed (byte* pBuffer = buffer)
            {
                return Load(pBuffer, (uint)buffer.Length);
            }
        }

        public Image Load(string identifier)
        {
            var fullPath = Titan.Core.IO.FileSystem.GetFullPath(identifier);

            using ComPtr<IWICBitmapDecoder> decoder = default;
            fixed (char* wzFilename = fullPath)
            {
                CheckAndThrow(_factory.Get()->CreateDecoderFromFilename(wzFilename, null, (uint)GENERIC_READ, WICDecodeMetadataCacheOnDemand, decoder.GetAddressOf()), nameof(IWICImagingFactory.CreateDecoderFromFilename));
            }
            return LoadInternal(decoder);
        }

        private Image LoadInternal(in ComPtr<IWICBitmapDecoder> decoder)
        {
            using ComPtr<IWICBitmapFrameDecode> frameDecode = default;
            CheckAndThrow(decoder.Get()->GetFrame(0, frameDecode.GetAddressOf()), nameof(IWICBitmapDecoder.GetFrame));

            Guid pixelFormat;
            CheckAndThrow(frameDecode.Get()->GetPixelFormat(&pixelFormat), nameof(IWICBitmapFrameDecode.GetPixelFormat));

            uint height, width;
            CheckAndThrow(frameDecode.Get()->GetSize(&width, &height), nameof(IWICBitmapFrameDecode.GetSize));

            var dxgiFormat = WICToDXGITranslationTable.Translate(pixelFormat);
            if (dxgiFormat == DXGI_FORMAT.DXGI_FORMAT_UNKNOWN)
            {
                // needs conversion
                var newFormatGuid = WICConvertionTable.Convert(pixelFormat);
                var newBitsPerPixel = GetBitsPerPixel(newFormatGuid);

                var rowPitch = (width * newBitsPerPixel + 7) / 8;
                var imageSize = rowPitch * height;

                var buffer = (byte*)NativeMemory.Alloc((nuint)imageSize);
                try
                {
                    using ComPtr<IWICFormatConverter> converter = default;
                    CheckAndThrow(_factory.Get()->CreateFormatConverter(converter.GetAddressOf()), nameof(IWICImagingFactory.CreateFormatConverter));
                    CheckAndThrow(converter.Get()->Initialize((IWICBitmapSource*)frameDecode.Get(), &newFormatGuid, WICBitmapDitherTypeErrorDiffusion, null, 0, WICBitmapPaletteTypeCustom), nameof(IWICFormatConverter.Initialize));
                    CheckAndThrow(converter.Get()->CopyPixels(null, rowPitch, imageSize, buffer), nameof(IWICFormatConverter.CopyPixels));

                    return new Image(buffer, imageSize, WICToDXGITranslationTable.Translate(newFormatGuid), rowPitch, width, height);
                }
                catch
                {
                    // If there's an exception, free the buffer
                    NativeMemory.Free(buffer);
                    throw;
                }
            }
            else
            {
                var bitsPerPixel = GetBitsPerPixel(pixelFormat);
                var stride = (width * bitsPerPixel + 7) / 8;
                var imageSize = stride * height;

                var buffer = (byte*)NativeMemory.Alloc((nuint)imageSize);
                try
                {
                    CheckAndThrow(frameDecode.Get()->CopyPixels(null, stride, imageSize, buffer), nameof(IWICBitmapFrameDecode.CopyPixels));
                }
                catch
                {
                    // If there's an exception, free the buffer
                    NativeMemory.Free(buffer);
                    throw;
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
