using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Shaders;

namespace Titan.Graphics.Shaders
{
    public interface IShaderLoader
    {
        VertexShader CreateVertexShader(VertexShaderDescriptor descriptor);
        (VertexShader vertexshader, InputLayout inputLayout) CreateVertexShaderAndInputLayout(VertexShaderDescriptor descriptor, in InputLayoutDescriptor[] layout);
        PixelShader CreatePixelShader(PixelShaderDescriptor descriptor);
    }
}
