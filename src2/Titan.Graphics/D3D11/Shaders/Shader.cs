using System.Runtime.InteropServices;
using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11.Shaders
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal unsafe struct Shader
    {
        public ID3D11VertexShader* VertexShader;
        public ID3D11PixelShader* PixelShader;
        public ID3D11InputLayout* InputLayout;

        public int Handle;
    }
}
