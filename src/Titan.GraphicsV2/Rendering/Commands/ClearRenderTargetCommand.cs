using System.Runtime.InteropServices;
using Titan.GraphicsV2.D3D11;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Rendering.Commands
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ClearRenderTargetCommand
    {
        internal RenderCommandTypes Type;
        internal ID3D11RenderTargetView* RenderTarget;
        internal fixed float Color[4];
        public ClearRenderTargetCommand(ID3D11RenderTargetView* renderTarget, in Color color)
        {
            Type = RenderCommandTypes.ClearRenderTarget;
            RenderTarget = renderTarget;
            Color[0] = color.R;
            Color[1] = color.G;
            Color[2] = color.B;
            Color[3] = color.A;
        }
    }
}
