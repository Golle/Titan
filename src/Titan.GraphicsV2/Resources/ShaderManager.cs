using System.IO;
using System.Linq;
using System.Text;
using Titan.Core.IO;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Shaders;
using Titan.GraphicsV2.Rendering;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Resources
{
    // TODO: Implement caching of input layouts and shaders
    internal unsafe class ShaderManager
    {
        private readonly ShaderCompiler _shaderCompiler;
        private readonly ShaderFactory _shaderFactory;
        private readonly InputLayoutFactory _inputLayoutFactory;
        private readonly IFileReader _fileReader;
        
        public ShaderManager(ShaderCompiler shaderCompiler, ShaderFactory shaderFactory, InputLayoutFactory inputLayoutFactory, IFileReader fileReader)
        {
            _shaderCompiler = shaderCompiler;
            _shaderFactory = shaderFactory;
            _inputLayoutFactory = inputLayoutFactory;
            _fileReader = fileReader;
        }

        public VertexShader Load(VertexShaderSpecification specification)
        {
            var code = _fileReader.ReadText(specification.Filename);
            var compiledShader = _shaderCompiler.Compile(code, specification.Entrypoint, specification.Version);
            var shader = _shaderFactory.CreateVertexShaderP(compiledShader);
            var inputLayout = _inputLayoutFactory.CreateP(compiledShader, specification.InputLayout.Select(i => new InputLayoutDescriptor(i.SemanticName, (DXGI_FORMAT)i.Format)).ToArray());
            return new(inputLayout, shader);
        }

        public PixelShader Load(PixelShaderSpecification specification)
        {
            var code = _fileReader.ReadText(specification.Filename);
            var compiledShader = _shaderCompiler.Compile(code, specification.Entrypoint, specification.Version);
            var shader = _shaderFactory.CreatePixelShaderP(compiledShader);
            
            return new(shader);
        }
    }
}
