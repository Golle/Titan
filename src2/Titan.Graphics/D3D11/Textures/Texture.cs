using System.Runtime.InteropServices;
using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11.Textures
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct Texture
    {
        internal ID3D11Texture2D* D3DTexture;
        internal ID3D11RenderTargetView* D3DTarget;
        internal ID3D11ShaderResourceView* D3DResource;
        
        internal uint Width;
        internal uint Height;

        internal int Handle; // TODO: internal Handle<Texture> causes a TypeLoadException in the C++ code :O
        internal DXGI_FORMAT Format;
        internal D3D11_BIND_FLAG BindFlags;
        internal D3D11_USAGE Usage;
    }
}