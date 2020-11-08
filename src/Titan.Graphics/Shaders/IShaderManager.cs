using System;
using Titan.Graphics.D3D11;

namespace Titan.Graphics.Shaders
{
    public record VertexShaderDescriptor(string Filename, string Version, string Entrypoint, ShaderDefines[] Defines = default);
    public record PixelShaderDescriptor(string Filename, string Version, string Entrypoint, ShaderDefines[] Defines = default);

    public interface IShaderManager : IDisposable
    {
        uint AddShaderProgram(string name, VertexShaderDescriptor vsDescriptor, PixelShaderDescriptor psDescriptor, in InputLayoutDescriptor[] inputLayoutDescriptor);
        uint GetHandle(string name);
        ShaderProgram Get(uint handle);
    }
}
