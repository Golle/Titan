using System;
using System.Runtime.CompilerServices;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.State;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;

namespace Titan.Graphics.D3D11
{
    public unsafe class ImmediateContext : IDisposable, IRenderContext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ID3D11DeviceContext* AsPointer() => Context.Get();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref readonly ComPtr<ID3D11DeviceContext> AsComPointer() => ref Context;

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
        public void ClearDepthStencilView(DepthStencilView depthStencilView)
        {
            Context.Get()->ClearDepthStencilView(depthStencilView.AsPointer(), 1, 1, 0);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShaderResource(ShaderResourceView resource, uint slot = 0) => Context.Get()->VSSetShaderResources(slot, 1u, resource.Ptr.GetAddressOf());

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

        public void MapResource<T>(ID3D11Resource* resource, in T value) where T : unmanaged
        {
            var context = Context.Get();
            D3D11_MAPPED_SUBRESOURCE mappedResource;
#if DEBUG
            CheckAndThrow(context->Map(resource, 0, D3D11_MAP.D3D11_MAP_WRITE_DISCARD, 0, &mappedResource), "Failed to Map Resource");
#else
            context->Map(resource, 0, D3D11_MAP.D3D11_MAP_WRITE_DISCARD, 0, &mappedResource);
#endif
            fixed (void* pData = &value)
            {
                var size = sizeof(T);
                Buffer.MemoryCopy(pData, mappedResource.pData, size, size);
            }
            context->Unmap(resource, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRenderTargets(ID3D11RenderTargetView** renderTargets, uint numViews, ID3D11DepthStencilView* depthStencilView) => Context.Get()->OMSetRenderTargets(numViews, renderTargets, depthStencilView);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderResources(ID3D11ShaderResourceView** resources, uint numViews, uint startSlot = 0u) => Context.Get()->PSSetShaderResources(startSlot, numViews, resources);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShaderResources(ID3D11ShaderResourceView** resources, uint numViews, uint startSlot = 0u) => Context.Get()->VSSetShaderResources(startSlot, numViews, resources);

        public void Dispose() => Context.Dispose();
    }
}
