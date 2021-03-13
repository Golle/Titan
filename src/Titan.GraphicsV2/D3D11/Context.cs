//using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.GraphicsV2.D3D11.Buffers;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
// ReSharper disable InconsistentNaming

namespace Titan.GraphicsV2.D3D11
{
    internal unsafe class Context
    {
        private readonly ID3D11DeviceContext* _context;
        public Context(ID3D11DeviceContext* context)
        {
            _context = context;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(uint vertexCount, uint startVertexLocation) => _context->Draw(vertexCount, startVertexLocation);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawIndexed(uint indexCount, uint startIndex = 0u, int baseVertexIndex = 0) => _context->DrawIndexed(indexCount, startIndex, baseVertexIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRenderTargets(uint count, ID3D11RenderTargetView** renderTarget, ID3D11DepthStencilView* depthStencilView) => _context->OMSetRenderTargets(count, renderTarget, depthStencilView);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTarget(ID3D11RenderTargetView* renderTarget, float* color) => _context->ClearRenderTargetView(renderTarget, color);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderResources(uint numberOfViews, ID3D11ShaderResourceView** resourceViews) => _context->PSSetShaderResources(0, numberOfViews, resourceViews);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShaderResources(uint numberOfViews, ID3D11ShaderResourceView** resourceViews) => _context->VSSetShaderResources(0, numberOfViews, resourceViews);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderSamplers(uint numberOfSamplers, ID3D11SamplerState** samplers) => _context->PSSetSamplers(0, numberOfSamplers, samplers);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShaderSamplers(uint numberOfSamplers, ID3D11SamplerState** samplers) => _context->VSSetSamplers(0, numberOfSamplers, samplers);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShader(ID3D11PixelShader* shader) => _context->PSSetShader(shader, null, 0u);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShader(ID3D11VertexShader* shader) => _context->VSSetShader(shader, null, 0);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetInputLayout(ID3D11InputLayout* inputLayout) => _context->IASetInputLayout(inputLayout);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTopology(in D3D_PRIMITIVE_TOPOLOGY topology) => _context->IASetPrimitiveTopology(topology);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexBuffer(in Buffer vertexBuffer)
        {
            Debug.Assert(vertexBuffer.BindFlag == D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER);
            
            var stride = vertexBuffer.Stride;
            var offset = 0u;
            fixed (ID3D11Buffer** ppBuffer = &vertexBuffer.Resource)
            {
                _context->IASetVertexBuffers(0, 1, ppBuffer, &stride, &offset);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetIndexBuffer(in Buffer indexBuffer)
        {
            Debug.Assert(indexBuffer.BindFlag == D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER);
            Debug.Assert(indexBuffer.Stride == 4u || indexBuffer.Stride == 2u, $"Stride {indexBuffer.Stride} is not supported. Only 2 or 4 is supported.");

            var format = indexBuffer.Stride switch
            {
                2u => DXGI_FORMAT.DXGI_FORMAT_R16_UINT,
                4u => DXGI_FORMAT.DXGI_FORMAT_R32_UINT,
                _ => DXGI_FORMAT.DXGI_FORMAT_R16_UINT // this will not happen, but makes the compiler happy
            };
            // TODO: add support for offset if needed
            _context->IASetIndexBuffer(indexBuffer.Resource, format, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetViewPort(in ViewPort viewPort)
        {
            fixed (D3D11_VIEWPORT* pViewPort = &viewPort.Resource)
            {
                _context->RSSetViewports(1, pViewPort);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVSConstantBuffer(in Buffer buffer, uint slot = 0)
        {
            Debug.Assert(buffer.BindFlag == D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER);
            var buffers = stackalloc ID3D11Buffer*[1];
            buffers[0] = buffer.Resource;

            _context->VSSetConstantBuffers(slot, 1, buffers);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPSConstantBuffer(in Buffer buffer, uint slot = 0)
        {
            Debug.Assert(buffer.BindFlag == D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER);
            var buffers = stackalloc ID3D11Buffer*[1];
            buffers[0] = buffer.Resource;

            _context->PSSetConstantBuffers(slot, 1, buffers);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Map<T>(in Buffer buffer, in T value) where T : unmanaged
        {
            D3D11_MAPPED_SUBRESOURCE subresource;
            var resource = (ID3D11Resource*) buffer.Resource;
            var result = _context->Map(resource, 0, D3D11_MAP.D3D11_MAP_WRITE_DISCARD, 0, &subresource);
#if DEBUG
            Common.CheckAndThrow(result, nameof(ID3D11DeviceContext.Map));
#endif
            var size = sizeof(T);
            fixed (T* pValue = &value)
            {
                System.Buffer.MemoryCopy(pValue, subresource.pData, size, size);
            }
            _context->Unmap(resource, 0);
        }
    }
}
