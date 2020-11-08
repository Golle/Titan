namespace Titan.Graphics.Shaders1
{
    public readonly struct ShaderProgram
    {
        public readonly VertexShaderHandle VertexShader;
        public readonly PixelShaderHandle PixelShader;
        public readonly InputLayoutHandle InputLayout;
        public ShaderProgram(in VertexShaderHandle vertexShaderHandle, in PixelShaderHandle pixelShaderHandle, in InputLayoutHandle inputLayoutHandle)
        {
            VertexShader = vertexShaderHandle;
            PixelShader = pixelShaderHandle;
            InputLayout = inputLayoutHandle;
        }
    }
}
