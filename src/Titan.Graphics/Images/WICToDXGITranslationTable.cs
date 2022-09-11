using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Titan.Platform.Win32.DXGI;
using static Titan.Platform.Win32.DXGI.DXGI_FORMAT;
using static Titan.Platform.Win32.WIC.WICPixelFormats;

// ReSharper disable InconsistentNaming

namespace Titan.Graphics.Images
{
    internal class WICToDXGITranslationTable
    {
        private static readonly Dictionary<Guid, DXGI_FORMAT> _table = new()
        {
            { GUID_WICPixelFormat128bppRGBAFloat, DXGI_FORMAT_R32G32B32A32_FLOAT },

            { GUID_WICPixelFormat64bppRGBAHalf, DXGI_FORMAT_R16G16B16A16_FLOAT },
            { GUID_WICPixelFormat64bppRGBA, DXGI_FORMAT_R16G16B16A16_UNORM },

            { GUID_WICPixelFormat32bppRGBA, DXGI_FORMAT_R8G8B8A8_UNORM },
            { GUID_WICPixelFormat32bppBGRA, DXGI_FORMAT_B8G8R8A8_UNORM }, // DXGI 1.1
            { GUID_WICPixelFormat32bppBGR, DXGI_FORMAT_B8G8R8X8_UNORM }, // DXGI 1.1

            { GUID_WICPixelFormat32bppRGBA1010102XR, DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM }, // DXGI 1.1
            { GUID_WICPixelFormat32bppRGBA1010102, DXGI_FORMAT_R10G10B10A2_UNORM },
            { GUID_WICPixelFormat32bppRGBE, DXGI_FORMAT_R9G9B9E5_SHAREDEXP },

            //#ifdef DXGI_1_2_FORMATSj

            { GUID_WICPixelFormat16bppBGRA5551, DXGI_FORMAT_B5G5R5A1_UNORM },
            { GUID_WICPixelFormat16bppBGR565, DXGI_FORMAT_B5G6R5_UNORM },

            //#endif // DXGI_1_2_FORMATS

            { GUID_WICPixelFormat32bppGrayFloat, DXGI_FORMAT_R32_FLOAT },
            { GUID_WICPixelFormat16bppGrayHalf, DXGI_FORMAT_R16_FLOAT },
            { GUID_WICPixelFormat16bppGray, DXGI_FORMAT_R16_UNORM },
            { GUID_WICPixelFormat8bppGray, DXGI_FORMAT_R8_UNORM },

            { GUID_WICPixelFormat8bppAlpha, DXGI_FORMAT_A8_UNORM },

            //#if (_WIN32_WINNT >= 0x0602 /*_WIN32_WINNT_WIN8*/)
            { GUID_WICPixelFormat96bppRGBFloat, DXGI_FORMAT_R32G32B32_FLOAT },
            //#endif

        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DXGI_FORMAT Translate(in Guid guid) => _table.TryGetValue(guid, out var format) ? format : DXGI_FORMAT_UNKNOWN;
    }
}
