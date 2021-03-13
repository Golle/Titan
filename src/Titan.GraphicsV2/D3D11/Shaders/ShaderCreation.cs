using System;

namespace Titan.GraphicsV2.D3D11.Shaders
{
    internal record ShaderCreation
    {
        internal ShaderDescription VertexShader { get; init; }
        internal ShaderDescription PixelShader { get; init; }
        internal InputLayoutDescription[] InputLayout { get; init; } = Array.Empty<InputLayoutDescription>();
    }
}
