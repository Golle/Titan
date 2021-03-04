using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.GraphicsV2.D3D11;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Rendering
{

    // TODO: is this better than the CommandBuffer? easier to debug, but will require a lot of if-statements in the render stage. 
    // better memory usage, single list with all pipeline stages in instead of a unmanaged heap allocated array for the buffer
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct SampleStage
    {
        internal bool ClearDepthStencil;
        internal bool ClearRenderTargets;
        internal Color ClearColor;
        internal float DepthStencilClearValue;


        internal ID3D11DepthStencilView* DepthStencilView;
        internal ID3D11PixelShader* PixelShader;
        internal ID3D11VertexShader* VertexShader;
        internal ID3D11InputLayout* InputLayout;


        internal uint NumberOfVertexShaderResources;
        internal ID3D11ShaderResourceView** VertexShaderResources => (ID3D11ShaderResourceView**)Unsafe.AsPointer(ref _vertexShaderResources);
        private FixedBuffer6 _vertexShaderResources;

        internal uint NumberOfPixelShaderResources;
        internal ID3D11ShaderResourceView** PixekShaderResources => (ID3D11ShaderResourceView**)Unsafe.AsPointer(ref _pixelShaderResources);
        private FixedBuffer6 _pixelShaderResources;


        internal uint NumberOfVertexShaderSamplers;
        internal ID3D11SamplerState** VertexShaderSamplers => (ID3D11SamplerState**)Unsafe.AsPointer(ref _vertexShaderSamplers);
        private FixedBuffer2 _vertexShaderSamplers;
        internal uint NumberOfPixelShaderSamplers;
        internal ID3D11SamplerState** PixelShaderSamplers => (ID3D11SamplerState**)Unsafe.AsPointer(ref _pixelShaderSamplers);
        private FixedBuffer2 _pixelShaderSamplers;


        internal uint NumberOfRenderTargets;
        internal ID3D11RenderTargetView** RenderTargets => (ID3D11RenderTargetView**)Unsafe.AsPointer(ref _internalRenderTarget);
        private FixedBuffer4 _internalRenderTarget;
    }


    [StructLayout(LayoutKind.Sequential, Size = sizeof(long) * 2)]
    internal readonly struct FixedBuffer2 { }
    [StructLayout(LayoutKind.Sequential, Size = sizeof(long) * 4)]
    internal readonly struct FixedBuffer4 { }

    [StructLayout(LayoutKind.Sequential, Size = sizeof(long) * 6)]
    internal readonly struct FixedBuffer6 { }
    
}

