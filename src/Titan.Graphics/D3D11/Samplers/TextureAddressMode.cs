using static Titan.Platform.Win32.D3D11.D3D11_TEXTURE_ADDRESS_MODE;

namespace Titan.Graphics.D3D11.Samplers
{
    public enum TextureAddressMode
    {
        Invalid,
        Wrap = D3D11_TEXTURE_ADDRESS_WRAP,
        Mirror  = D3D11_TEXTURE_ADDRESS_MIRROR,
        Clamp = D3D11_TEXTURE_ADDRESS_CLAMP,
        Border = D3D11_TEXTURE_ADDRESS_BORDER,
        MirrorOnce = D3D11_TEXTURE_ADDRESS_MIRROR_ONCE,
    }
}
