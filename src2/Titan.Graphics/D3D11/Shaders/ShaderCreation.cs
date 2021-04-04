using System;

namespace Titan.Graphics.D3D11.Shaders
{
    internal record ShaderCreation
    {
        internal ShaderDescription VertexShader { get; init; }
        internal ShaderDescription PixelShader { get; init; }
        internal InputLayoutDescription[] InputLayout { get; init; } = Array.Empty<InputLayoutDescription>();
    }
}
