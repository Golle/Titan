using Titan.Core;
using Titan.Graphics.D3D11.BlendStates;
using Titan.Graphics.D3D11.Rasterizer;
using Titan.Graphics.D3D11.Samplers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Graphics.D3D11.Pipeline;

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
    public Handle<BlendState> BlendState;
    public Handle<RasterizerState> RasterizerState;

    public Renderer Renderer;
}

// this might be something we can use
//[StructLayout(LayoutKind.Sequential)]
//public unsafe struct HandleVector10<T> where T : unmanaged
//{
//    private fixed uint Handles[10];
//    public readonly uint Size;
//    public ReadOnlySpan<Handle<T>> Get() => new(Unsafe.AsPointer(ref this), (int)Size);
//    public HandleVector10(Handle<T>[] handles)
//    {
//        Size = (uint)handles.Length;
//        fixed (Handle<T>* pHandles = handles)
//        fixed (uint* pDestination = Handles)
//        {
//            Unsafe.CopyBlockUnaligned(pDestination, pHandles, sizeof(uint) * Size);
//        }
//    }
//}
