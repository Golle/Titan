using Titan.Graphics.D3D11;
using Titan.Graphics.Shaders;

namespace Titan.Graphics.Pipeline.Configuration
{
    public class PipelineConfiguration
    {
        public ShaderProgramConfiguration[] ShaderPrograms { get; set; }
    }

    public class ShaderProgramConfiguration
    {
        public string Name { get; set; }
        public VertexShaderDescriptor VertexShader { get; set; }
        public PixelShaderDescriptor PixelShader { get; set; }
        public InputLayoutDescriptor[] Layout { get; set; }
    }

    
}
