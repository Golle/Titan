using System.Runtime.InteropServices;
using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11.Shaders
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal unsafe struct Shader
    {
        internal ID3D11VertexShader* VertexShader;
        internal ID3D11PixelShader* PixelShader;
        internal ID3D11InputLayout* InputLayout;

        internal int Handle;
    }
}
