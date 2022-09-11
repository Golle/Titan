using Titan.Platform.Win32.D3D11;

namespace Titan.Graphics.D3D11.Shaders;

public unsafe struct PixelShader
{
    public int Handle;
    public ID3D11PixelShader* Shader;
}
