using System.Runtime.InteropServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Rendering.Commands
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SetShadersCommand
    {
        internal RenderCommandTypes Type;
        internal ID3D11PixelShader* PixelShader;
        internal ID3D11VertexShader* VertexShader;
        internal ID3D11InputLayout* InputLayout;

        public SetShadersCommand(ID3D11VertexShader* vertexShader, ID3D11PixelShader* pixelShader, ID3D11InputLayout* inputLayout)
        {
            VertexShader = vertexShader;
            PixelShader = pixelShader;
            InputLayout = inputLayout;
            Type = RenderCommandTypes.SetShaders;
        }
    }
}
