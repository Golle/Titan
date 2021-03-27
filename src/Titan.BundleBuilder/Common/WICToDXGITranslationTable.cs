using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Titan.Windows.Win32.D3D11;
using Titan.Windows.Win32.WIC;

namespace Titan.BundleBuilder.Common
{
    internal class WICToDXGITranslationTable
    {
        private static readonly Dictionary<Guid, DXGI_FORMAT> Table = new()
        {
            { WICPixelFormats.GUID_WICPixelFormat128bppRGBAFloat, DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT },

            { WICPixelFormats.GUID_WICPixelFormat64bppRGBAHalf, DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_FLOAT },
            { WICPixelFormats.GUID_WICPixelFormat64bppRGBA, DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_UNORM },

            { WICPixelFormats.GUID_WICPixelFormat32bppRGBA, DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM },
            { WICPixelFormats.GUID_WICPixelFormat32bppBGRA, DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM }, // DXGI 1.1
            { WICPixelFormats.GUID_WICPixelFormat32bppBGR, DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM }, // DXGI 1.1

            { WICPixelFormats.GUID_WICPixelFormat32bppRGBA1010102XR, DXGI_FORMAT.DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM }, // DXGI 1.1
            { WICPixelFormats.GUID_WICPixelFormat32bppRGBA1010102, DXGI_FORMAT.DXGI_FORMAT_R10G10B10A2_UNORM },
            { WICPixelFormats.GUID_WICPixelFormat32bppRGBE, DXGI_FORMAT.DXGI_FORMAT_R9G9B9E5_SHAREDEXP },

            //#ifdef DXGI_1_2_FORMATSj

            { WICPixelFormats.GUID_WICPixelFormat16bppBGRA5551, DXGI_FORMAT.DXGI_FORMAT_B5G5R5A1_UNORM },
            { WICPixelFormats.GUID_WICPixelFormat16bppBGR565, DXGI_FORMAT.DXGI_FORMAT_B5G6R5_UNORM },

            //#endif // DXGI_1_2_FORMATS

            { WICPixelFormats.GUID_WICPixelFormat32bppGrayFloat, DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT },
            { WICPixelFormats.GUID_WICPixelFormat16bppGrayHalf, DXGI_FORMAT.DXGI_FORMAT_R16_FLOAT },
            { WICPixelFormats.GUID_WICPixelFormat16bppGray, DXGI_FORMAT.DXGI_FORMAT_R16_UNORM },
            { WICPixelFormats.GUID_WICPixelFormat8bppGray, DXGI_FORMAT.DXGI_FORMAT_R8_UNORM },

            { WICPixelFormats.GUID_WICPixelFormat8bppAlpha, DXGI_FORMAT.DXGI_FORMAT_A8_UNORM },

            //#if (_WIN32_WINNT >= 0x0602 /*_WIN32_WINNT_WIN8*/)
            { WICPixelFormats.GUID_WICPixelFormat96bppRGBFloat, DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT },
            //#endif

        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DXGI_FORMAT Translate(in Guid guid) => Table.TryGetValue(guid, out var format) ? format : DXGI_FORMAT.DXGI_FORMAT_UNKNOWN;
    }
}