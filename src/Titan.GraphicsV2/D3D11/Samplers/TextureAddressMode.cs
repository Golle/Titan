﻿using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11.Samplers
{
    internal enum TextureAddressMode
    {
        Wrap = D3D11_TEXTURE_ADDRESS_MODE.D3D11_TEXTURE_ADDRESS_WRAP,
        Mirror  = D3D11_TEXTURE_ADDRESS_MODE.D3D11_TEXTURE_ADDRESS_MIRROR,
        Clamp = D3D11_TEXTURE_ADDRESS_MODE.D3D11_TEXTURE_ADDRESS_CLAMP,
        Border = D3D11_TEXTURE_ADDRESS_MODE.D3D11_TEXTURE_ADDRESS_BORDER,
        MirrorOnce = D3D11_TEXTURE_ADDRESS_MODE.D3D11_TEXTURE_ADDRESS_MIRROR_ONCE,
    }
}
