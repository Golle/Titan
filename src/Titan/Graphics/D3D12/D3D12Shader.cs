using Titan.Graphics.Resources;
using Titan.Platform.Win32.D3D12;

namespace Titan.Graphics.D3D12;

internal struct D3D12Shader
{
    public Shader Shader;
    // Add additional D3D12 stuff if needed.
    public D3D12_SHADER_BYTECODE ByteCode;
}
