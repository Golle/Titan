using Titan.Core;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Graphics.D3D11.Pipeline
{
    public struct Pipeline
    {
        public bool ClearRenderTargets;
        public bool ClearDepthStencil;

        public Handle<PixelShader> PixelShader;
        public Handle<VertexShader> VertexShader;


        public Color ClearColor;

        public Handle<Texture>[] PixelShaderResources; // PS Input
        public Handle<Texture>[] VertexShaderResources; // VS Input
        public Handle<Texture>[] RenderTargets; // Output


        //public int NumberOfRenderTargets;
        //public unsafe ID3D11RenderTargetView** RenderTargetsCache => (ID3D11RenderTargetView**)Unsafe.AsPointer(ref _renderTargetsCache);
        //private FixedBuffer4 _renderTargetsCache;


    }
    
    //[StructLayout(LayoutKind.Sequential, Size = sizeof(long)*4)]
    //internal struct FixedBuffer4{}
}
