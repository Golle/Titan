using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Graphics.D3D11.Samplers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Graphics.D3D11.Pipeline
{
    public struct Pipeline
    {
        public bool ClearRenderTargets;
        public bool ClearDepthBuffer;
        public float DepthBufferClearValue;

        public Handle<PixelShader> PixelShader;
        public Handle<VertexShader> VertexShader;


        public Color ClearColor;

        public Handle<Texture>[] PixelShaderResources; // PS Input
        public Handle<Texture>[] VertexShaderResources; // VS Input
        public Handle<Texture>[] RenderTargets; // Output
        public Handle<Sampler>[] PixelShaderSamplers;
        public Handle<Sampler>[] VertexShaderSamplers;
        public Handle<Texture> DepthBuffer;

        public Renderer Renderer;


        

        // TODO: this can be used to microoptimize the binding of render targets

        // About 15-20% increase in performance (in reality, 10-12 ticks)
        //public uint NumberOfRenderTargets;
        //public unsafe ID3D11RenderTargetView** RenderTargetsCache => (ID3D11RenderTargetView**)Unsafe.AsPointer(ref _renderTargetsCache);
        //private FixedBuffer4 _renderTargetsCache;
        //public unsafe void UpdateCached()
        //{
        //    for (var i = 0; i < RenderTargets.Length; ++i)
        //    {
        //        RenderTargetsCache[i] = GraphicsDevice.TextureManager.Access(RenderTargets[i]).D3DTarget;
        //    }
        //    NumberOfRenderTargets = (uint) RenderTargets.Length;
        //}
    }

    [StructLayout(LayoutKind.Sequential, Size = sizeof(long)*4)]
    internal struct FixedBuffer4{}
}
