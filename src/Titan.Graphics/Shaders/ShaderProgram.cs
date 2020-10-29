using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Shaders;

namespace Titan.Graphics.Shaders
{
    public class ShaderProgram
    {
        public string Name { get; }
        private readonly InputLayout _inputLayout;
        private readonly VertexShader _vertexShader;
        private readonly PixelShader _pixelShader;

        public ShaderProgram(string name, InputLayout inputLayout, VertexShader vertexShader, PixelShader pixelShader)
        {
            Name = name;
            _inputLayout = inputLayout;
            _vertexShader = vertexShader;
            _pixelShader = pixelShader;
        }

        public void Bind(ImmediateContext context)
        {
            context.SetInputLayout(_inputLayout);
            context.SetPixelShader(_pixelShader);
            context.SetVertexShader(_vertexShader);
        }
    }
}
