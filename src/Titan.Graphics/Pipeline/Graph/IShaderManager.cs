using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Shaders;

namespace Titan.Graphics.Pipeline.Graph
{
    public interface IShaderManager
    {
        (VertexShader vertexshader, InputLayout inputLayout) GetVertexShader(string filename, in InputLayoutDescriptor[] layout);
        PixelShader GetPixelShader(string filename);
    }

    internal class ShaderManager : IShaderManager
    {
        private readonly IGraphicsDevice _device;
        private readonly IShaderCompiler _shaderCompiler;

        public ShaderManager(IGraphicsDevice device, IShaderCompiler shaderCompiler)
        {
            _device = device;
            _shaderCompiler = shaderCompiler;
        }

        public (VertexShader vertexshader, InputLayout inputLayout) GetVertexShader(string filename, in InputLayoutDescriptor[] layout)
        {
            using var shader = _shaderCompiler.CompileShaderFromFile(filename, "main", "vs_5_0");// try to retrieve this from the cache
            var inputLayout = new InputLayout(_device, shader, layout); // try to retrieve this from the cache

            var vertexShader = new VertexShader(_device, shader);
            return (vertexShader, inputLayout);
        }

        public PixelShader GetPixelShader(string filename)
        {
            var shader = _shaderCompiler.CompileShaderFromFile(filename, "main", "ps_5_0");
            
            return new PixelShader(_device, shader);
        }
    }
}
