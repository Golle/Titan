using System;

namespace Titan.Graphics.Shaders
{
    public record VertexShaderDescriptor(string Filename, string Version, string Entrypoint, ShaderDefines[] Defines = default);
    public record PixelShaderDescriptor(string Filename, string Version, string Entrypoint, ShaderDefines[] Defines = default);

    public interface IShaderManager : IDisposable
    {
        void Initialize(IGraphicsDevice graphicsDevice, string assetsPath);
        ShaderProgram GetByName(string name);
        ShaderProgram AddShader(string name, in VertexShaderDescriptor vertexShaderDescriptor, in PixelShaderDescriptor pixelShaderDescriptor, in InputLayoutDescriptor[] layout);
        ref readonly VertexShader this[in VertexShaderHandle handle] { get; }
        ref readonly PixelShader this[in PixelShaderHandle handle] { get; }
        ref readonly InputLayout this[in InputLayoutHandle handle] { get; }
    }
}
