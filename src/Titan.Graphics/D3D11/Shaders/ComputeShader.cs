using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11.Shaders;
public unsafe struct ComputeShader
{
    public int Handle;
    public ID3D11ComputeShader* Shader;
}
