using System;
using System.Runtime.CompilerServices;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.State;
using Titan.Graphics.Resources;
using Titan.Graphics.States;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;

namespace Titan.Graphics.D3D11
{
    public unsafe class RenderContext : IDisposable, IRenderContext
    {
        private ComPtr<ID3D11DeviceContext> _context;
        public RenderContext(ID3D11DeviceContext* context)
        {
            _context = new ComPtr<ID3D11DeviceContext>(context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTargetView(in RenderTargetView renderTargetView, in Color color)
        {
            fixed (Color* ptr = &color)
            {
                _context.Get()->ClearRenderTargetView(renderTargetView.Pointer, (float*)ptr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearDepthStencilView(in DepthStencilView depthStencilView) => _context.Get()->ClearDepthStencilView(depthStencilView.Pointer, 1, 1, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetViewport(Viewport viewport) => _context.Get()->RSSetViewports(1, (D3D11_VIEWPORT*)&viewport);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBlendState(BlendState blendState) => _context.Get()->OMSetBlendState(blendState.Ptr.Get(), blendState.BlendFactor, blendState.SampleMask);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShaderConstantBuffer(in ConstantBuffer constantBuffer, uint slot = 0)
        {
            fixed (ID3D11Buffer** pBuffer = &constantBuffer.Pointer)
            {
                _context.Get()->VSSetConstantBuffers(slot, 1, pBuffer);
            }
        } 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderConstantBuffer(in ConstantBuffer constantBuffer, uint slot = 0)
        {
            fixed (ID3D11Buffer** pBuffer = &constantBuffer.Pointer)
            {
                _context.Get()->PSSetConstantBuffers(slot, 1, pBuffer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexBuffer(in VertexBuffer vertexBuffer, uint slot = 0u, uint offset = 0u)
        {
            var stride = vertexBuffer.Stride;
            fixed (ID3D11Buffer** pBuffer = &vertexBuffer.Pointer)
            {
                _context.Get()->IASetVertexBuffers(slot, 1, pBuffer, &stride, &offset);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetIndexBuffer(in IndexBuffer indexBuffer, uint offset = 0u) => _context.Get()->IASetIndexBuffer(indexBuffer.Pointer, indexBuffer.Format, offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRenderTarget(in RenderTargetView renderTarget)
        {
            fixed (ID3D11RenderTargetView** pTarget = &renderTarget.Pointer)
            {
                _context.Get()->OMSetRenderTargets(1u, pTarget, null);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRenderTarget(in RenderTargetView renderTarget, in DepthStencilView depthStencilView)
        {
            fixed (ID3D11RenderTargetView** pTarget = &renderTarget.Pointer)
            {
                _context.Get()->OMSetRenderTargets(1u, pTarget, depthStencilView.Pointer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDepthStencilState(in DepthStencilState depthStencilState) => _context.Get()->OMSetDepthStencilState(depthStencilState.Pointer, 1u);

        // TODO: add methods for multiple shader resources in a single call
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderResource(in ShaderResourceView resource, uint slot = 0u)
        {
            fixed (ID3D11ShaderResourceView** pResource = &resource.Pointer)
            {
                _context.Get()->PSSetShaderResources(slot, 1u, pResource);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShaderResource(in ShaderResourceView resource, uint slot = 0)
        {
            fixed (ID3D11ShaderResourceView** pResource = &resource.Pointer)
            {
                _context.Get()->VSSetShaderResources(slot, 1u, pResource);
            }
        }

        // TODO: add methods for multiple samplers in a single call
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderSampler(SamplerState samplerState, uint slot = 0u) => _context.Get()->PSSetSamplers(slot, 1, samplerState.Ptr.GetAddressOf());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShaderSampler(SamplerState samplerState, uint slot = 0u) => _context.Get()->VSSetSamplers(slot, 1, samplerState.Ptr.GetAddressOf());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShader(PixelShader pixelShader) => _context.Get()->PSSetShader(pixelShader.Ptr.Get(), null, 0);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShader(VertexShader vertexShader) => _context.Get()->VSSetShader(vertexShader.Ptr.Get(), null, 0);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY topology) => _context.Get()->IASetPrimitiveTopology(topology);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetInputLayout(InputLayout inputLayout) => _context.Get()->IASetInputLayout(inputLayout.Pointer.Get());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawIndexed(uint indexCount, uint startIndexLocation = 0u, int baseVertexLocation = 0) => _context.Get()->DrawIndexed(indexCount, startIndexLocation, baseVertexLocation);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawIndexedInstanced(uint indexCountPerInstance, uint instanceCount, uint startIndexLocation = 0u, int baseVertexLocation = 0, uint startInstanceLocation = 0u) => _context.Get()->DrawIndexedInstanced(indexCountPerInstance, instanceCount, startIndexLocation, baseVertexLocation, startInstanceLocation);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteCommandList(in CommandList commandList, bool restoreContextState = false) => _context.Get()->ExecuteCommandList(commandList.Ptr, restoreContextState ? 1 : 0);

        public void MapResource<T>(ID3D11Resource* resource, in T value) where T : unmanaged
        {
            var context = _context.Get();
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
        public void SetRenderTargets(ID3D11RenderTargetView** renderTargets, uint numViews, ID3D11DepthStencilView* depthStencilView) => _context.Get()->OMSetRenderTargets(numViews, renderTargets, depthStencilView);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderResources(ID3D11ShaderResourceView** resources, uint numViews, uint startSlot = 0u) => _context.Get()->PSSetShaderResources(startSlot, numViews, resources);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShaderResources(ID3D11ShaderResourceView** resources, uint numViews, uint startSlot = 0u) => _context.Get()->VSSetShaderResources(startSlot, numViews, resources);

        public void Dispose() => _context.Dispose();
    }

}
