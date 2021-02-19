using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11.Shaders
{
    internal unsafe struct Shader
    {
        internal ID3D11VertexShader* VertexShader;
        internal ID3D11PixelShader* PixelShader;
        internal ID3D11InputLayout* InputLayout;

        internal int Handle;
    }
}
