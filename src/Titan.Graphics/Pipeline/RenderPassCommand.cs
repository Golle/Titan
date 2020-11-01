using System.Diagnostics;
using System.Runtime.InteropServices;
using Titan.Graphics.D3D11;
using Titan.Graphics.Pipeline.Renderers;
using Titan.Graphics.Shaders;

namespace Titan.Graphics.Pipeline
{
    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("{Type}")]
    internal struct RenderPassCommand
    {
        [FieldOffset(0)]
        public CommandType Type;
        [FieldOffset(8)]
        public RenderTargetView RenderTarget;
        [FieldOffset(8)]
        public ShaderProgram ShaderProgram;
        [FieldOffset(8)]
        public DepthStencilView DepthStencil;
        [FieldOffset(8)] 
        public ShaderResourceView ShaderResourceView;
        [FieldOffset(8)]
        public IRenderer Renderer;
        [FieldOffset(8)]
        public ClearRenderTargetCommand ClearRenderTarget;
        [FieldOffset(16)]
        public SetMultipleRenderTargetViewCommand MultipleRenderTargets;
    }
}
