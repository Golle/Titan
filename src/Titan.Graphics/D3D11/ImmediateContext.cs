using System;
using System.Runtime.CompilerServices;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.State;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11
{
    public unsafe class ImmediateContext : IDisposable
    {
        protected ComPtr<ID3D11DeviceContext> Context;
        


        protected ImmediateContext() { }

        public ImmediateContext(IGraphicsDevice device) => Context = new ComPtr<ID3D11DeviceContext>(device.ImmediateContextPtr);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTargetView(RenderTargetView renderTargetView, in Color color)
        {
            fixed (Color* ptr = &color)
            {
                Context.Get()->ClearRenderTargetView(renderTargetView.Ptr.Get(), (float*)ptr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetViewport(Viewport viewport) => Context.Get()->RSSetViewports(1, (D3D11_VIEWPORT*)&viewport);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBlendState(BlendState blendState) => Context.Get()->OMSetBlendState(blendState.Ptr.Get(), blendState.BlendFactor, blendState.SampleMask);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShaderConstantBuffer(IConstantBuffer constantBuffer, uint slot = 0) => Context.Get()->VSSetConstantBuffers(slot, 1, constantBuffer.Ptr.GetAddressOf());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderConstantBuffer(IConstantBuffer constantBuffer, uint slot = 0) => Context.Get()->PSSetConstantBuffers(slot, 1, constantBuffer.Ptr.GetAddressOf());

        // TODO: is this the best way to do it? 
        // TODO: Add support for multiple vertex buffers in a single call (same behavior can be achieved by calling this method and increase the slot, but it requires multiple calls instead of a single call)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexBuffer(IVertexBuffer vertexBuffer, uint slot = 0u, uint offset = 0u)
        {
            var stride = vertexBuffer.Stride;
            Context.Get()->IASetVertexBuffers(slot, 1, vertexBuffer.Buffer.GetAddressOf(), &stride, &offset);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetIndexBuffer(IIndexBuffer indexBuffer, uint offset = 0u) => Context.Get()->IASetIndexBuffer(indexBuffer.Buffer.Get(), indexBuffer.Format, offset);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRenderTarget(RenderTargetView renderTarget, DepthStencilView depthStencilView) => Context.Get()->OMSetRenderTargets(1u, renderTarget.Ptr.GetAddressOf(), depthStencilView.Ptr.Get());


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDepthStencilState(DepthStencilState depthStencilState) => Context.Get()->OMSetDepthStencilState(depthStencilState.Ptr.Get(), 1u);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRenderTarget(RenderTargetView renderTarget) => Context.Get()->OMSetRenderTargets(1u, renderTarget.Ptr.GetAddressOf(), null);
        // TODO: add methods for multiple shader resources in a single call
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderResource(ShaderResourceView resource, uint slot = 0u) => Context.Get()->PSSetShaderResources(slot, 1u, resource.Ptr.GetAddressOf());
        // TODO: add methods for multiple samplers in a single call
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderSampler(SamplerState samplerState, uint slot = 0u) => Context.Get()->PSSetSamplers(slot, 1, samplerState.Ptr.GetAddressOf());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShaderSampler(SamplerState samplerState, uint slot = 0u) => Context.Get()->VSSetSamplers(slot, 1, samplerState.Ptr.GetAddressOf());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShader(PixelShader pixelShader) => Context.Get()->PSSetShader(pixelShader.Ptr.Get(), null, 0);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShader(VertexShader vertexShader) => Context.Get()->VSSetShader(vertexShader.Ptr.Get(), null, 0);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY topology) => Context.Get()->IASetPrimitiveTopology(topology);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetInputLayout(InputLayout inputLayout) => Context.Get()->IASetInputLayout(inputLayout.Pointer.Get());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawIndexed(uint indexCount, uint startIndexLocation = 0u, int baseVertexLocation = 0) => Context.Get()->DrawIndexed(indexCount, startIndexLocation, baseVertexLocation);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawIndexedInstanced(uint indexCountPerInstance, uint instanceCount, uint startIndexLocation = 0u, int baseVertexLocation = 0, uint startInstanceLocation = 0u) => Context.Get()->DrawIndexedInstanced(indexCountPerInstance, instanceCount, startIndexLocation, baseVertexLocation, startInstanceLocation);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteCommandList(in CommandList commandList, bool restoreContextState = false) => Context.Get()->ExecuteCommandList(commandList.Ptr, restoreContextState ? 1 : 0);
        public void Dispose() => Context.Dispose();
    }
}
