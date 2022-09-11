using System.Runtime.InteropServices;
using Titan.Platform.Win32.D3D11;

namespace Titan.Graphics.D3D11.Textures
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct Texture
    {
        public ID3D11Texture2D* D3DTexture;
        public ID3D11RenderTargetView* D3DTarget;
        public ID3D11ShaderResourceView* D3DResource;
        public ID3D11DepthStencilView* D3DDepthStencil;

        public uint Width;
        public uint Height;

        public TextureFormats Format;
        public DepthStencilFormats DepthStencilFormat;

        public int Handle; // TODO: internal Handle<Texture> causes a TypeLoadException in the C++ code :O

        public D3D11_BIND_FLAG BindFlags;
        public D3D11_USAGE Usage;
        public D3D11_CPU_ACCESS_FLAG CpuAccess;
    }
}
