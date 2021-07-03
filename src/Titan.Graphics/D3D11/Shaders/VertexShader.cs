using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11.Shaders
{
    public unsafe struct VertexShader
    {
        public int Handle;
        public ID3D11VertexShader* Shader;
        public ID3D11InputLayout* InputLayout;
    }
}
