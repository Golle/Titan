using System.Diagnostics;
using Titan.Graphics.Pipeline.Renderers;
using Titan.Graphics.Resources;
using Titan.Graphics.Shaders;

namespace Titan.Graphics.Pipeline
{
    //[StructLayout(LayoutKind.Explicit)]// TODO: make this Explicit when all properties are handles
    [DebuggerDisplay("{Type}")]
    internal struct RenderPassCommand
    {
        //[FieldOffset(0)]
        public CommandType Type;
        //[FieldOffset(8)]
        public RenderTargetViewHandle RenderTarget;
        //[FieldOffset(8)]
        public ShaderProgram ShaderProgram;
        //[FieldOffset(8)]
        public DepthStencilViewHandle DepthStencil;
        //[FieldOffset(16)] 
        public SetShaderResourceCommand ShaderResource;
        //[FieldOffset(8)]
        public SetSamplerStateCommand SamplerState;
        //[FieldOffset(8)]
        public IRenderer Renderer;
        //[FieldOffset(8)]
        public ClearRenderTargetCommand ClearRenderTarget;
        //[FieldOffset(16)]
        public SetMultipleRenderTargetViewCommand MultipleRenderTargets;
        //[FieldOffset(16)]
        public uint Count;
    }
}
