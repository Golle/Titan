using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics.D3D11.BlendStates;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Rasterizer;
using Titan.Graphics.D3D11.Samplers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D;
using Titan.Platform.Win32.D3D11;
using Titan.Platform.Win32.DXGI;

namespace Titan.Graphics.D3D11
{
    public unsafe class Context
    {
        private readonly ID3D11DeviceContext* _context;

        public ID3D11DeviceContext* D3dContext => _context;
        public Context(ID3D11DeviceContext* context)
        {
            _context = context;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearRenderTarget(in Handle<Texture> handle, in Color color)
        {
            ref readonly var texture = ref GraphicsDevice.TextureManager.Access(handle);

            Debug.Assert((texture.BindFlags & D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET) != 0, "The texture is not a render target.");
            fixed (Color* pColor = &color)
            {
                _context->ClearRenderTargetView(texture.D3DTarget, (float*)pColor);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Map(in Handle<ResourceBuffer> handle, void * data, uint size)
        {
            D3D11_MAPPED_SUBRESOURCE subresource;
            ref readonly var buffer = ref GraphicsDevice.BufferManager.Access(handle);
            var resource = (ID3D11Resource*)buffer.Resource;
            var result = _context->Map(resource, 0, D3D11_MAP.D3D11_MAP_WRITE_DISCARD, 0, &subresource);
#if DEBUG
            Common.CheckAndThrow(result, nameof(ID3D11DeviceContext.Map));
#endif
            System.Buffer.MemoryCopy(data, subresource.pData, size, size);
            _context->Unmap(resource, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Map<T>(in Handle<ResourceBuffer> handle, in T value) where T : unmanaged
        {
            D3D11_MAPPED_SUBRESOURCE subresource;
            ref readonly var buffer = ref GraphicsDevice.BufferManager.Access(handle);
            var resource = (ID3D11Resource*)buffer.Resource;
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderSamplers(in Handle<Sampler>[] handles, uint startSlot = 0u)
        {
            if (handles == null)
            {
                return;
            }
            var numberOfSamplers = handles.Length;
            if (numberOfSamplers == 1)
            {
                SetPixelShaderSampler(handles[0], startSlot);
            }
            else
            {
                var samplerStates = stackalloc ID3D11SamplerState*[numberOfSamplers];
                for (var i = 0; i < numberOfSamplers; ++i)
                {
                    samplerStates[i] = GraphicsDevice.SamplerManager.Access(handles[i]).SamplerState;
                }
                _context->PSSetSamplers(startSlot, (uint)numberOfSamplers, samplerStates);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShaderSamplers(in Handle<Sampler>[] handles, uint startSlot = 0u)
        {
            if (handles == null)
            {
                return;
            }
            var numberOfSamplers = handles.Length;
            if (numberOfSamplers == 1)
            {
                SetVertexShaderSampler(handles[0], startSlot);
            }
            else
            {
                var samplerStates = stackalloc ID3D11SamplerState*[numberOfSamplers];
                for (var i = 0; i < numberOfSamplers; ++i)
                {
                    samplerStates[i] = GraphicsDevice.SamplerManager.Access(handles[i]).SamplerState;
                }
                _context->VSSetSamplers(startSlot, (uint)numberOfSamplers, samplerStates);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderSampler(in Handle<Sampler> handle, uint slot = 0u)
        {
            ref readonly var sampler = ref GraphicsDevice.SamplerManager.Access(handle);
            var samplerState = sampler.SamplerState;
            _context->PSSetSamplers(slot, 1, &samplerState);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShaderSampler(in Handle<Sampler> handle, uint slot = 0u)
        {
            ref readonly var sampler = ref GraphicsDevice.SamplerManager.Access(handle);
            var samplerState = sampler.SamplerState;
            _context->VSSetSamplers(slot, 1, &samplerState);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRenderTargets(in Handle<Texture>[] handles, in Handle<Texture> depthBufferHandle)
        {
            var depthBuffer = depthBufferHandle.IsValid() ? GraphicsDevice.TextureManager.Access(depthBufferHandle).D3DDepthStencil : null;
            var numberOfViews = handles.Length;
            if (numberOfViews == 1)
            {
                var renderTarget = GraphicsDevice.TextureManager.Access(handles[0]).D3DTarget;
                _context->OMSetRenderTargets(1, &renderTarget, depthBuffer);
            }
            else
            {
                var renderTargets = stackalloc ID3D11RenderTargetView*[numberOfViews];
                for (var i = 0; i < numberOfViews; ++i)
                {
                    renderTargets[i] = GraphicsDevice.TextureManager.Access(handles[i]).D3DTarget;
                }
                _context->OMSetRenderTargets((uint) numberOfViews, renderTargets, depthBuffer);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderResource(in Handle<Texture> handle, uint startSlot = 0u)
        {
            if (!handle.IsValid())
            {
                return;
            }
            var resource = GraphicsDevice.TextureManager.Access(handle).D3DResource;
            _context->PSSetShaderResources(startSlot, 1, &resource);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderResources(in ReadOnlySpan<Handle<Texture>> handles, uint startSlot = 0u)
        {
            if (handles == null)
            {
                return;
            }
            // TODO: might be faster to check if its just 1 in the array, and skip the stackalloc.
            var numberOfViews = handles.Length;
            var resources = stackalloc ID3D11ShaderResourceView*[numberOfViews];
            for (var i = 0; i < numberOfViews; ++i)
            {
                resources[i] = GraphicsDevice.TextureManager.Access(handles[i]).D3DResource;
            }
            _context->PSSetShaderResources(startSlot, (uint)numberOfViews, resources);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShaderResources(in Handle<Texture>[] handles, uint startSlot = 0u)
        {
            if (handles == null)
            {
                return;
            }
            // TODO: might be faster to check if its just 1 in the array, and skip the stackalloc.
            var numberOfViews = handles.Length;
            var resources = stackalloc ID3D11ShaderResourceView*[numberOfViews];
            for (var i = 0; i < numberOfViews; ++i)
            {
                resources[i] = GraphicsDevice.TextureManager.Access(handles[i]).D3DResource;
            }
            _context->VSSetShaderResources(startSlot, (uint) numberOfViews, resources);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnbindRenderTargets()
        {
            ID3D11RenderTargetView* renderTarget = null;
            _context->OMSetRenderTargets(1, &renderTarget, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShader(in Handle<PixelShader> handle) => _context->PSSetShader(GraphicsDevice.ShaderManager.Access(handle).Shader, null, 0u);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShader(in Handle<VertexShader> handle)
        {
            ref readonly var vertexShader = ref GraphicsDevice.ShaderManager.Access(handle);
            _context->VSSetShader(vertexShader.Shader, null, 0u);
            _context->IASetInputLayout(vertexShader.InputLayout);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnbindPixelShaderResources(ReadOnlySpan<Handle<Texture>> handles)
        {
            if (handles == null)
            {
                return;
            }

            var numberOfResources = handles.Length;
            var resources = stackalloc ID3D11ShaderResourceView*[numberOfResources];
            MemoryUtils.Init(resources, sizeof(ID3D11ShaderResourceView*)*numberOfResources);
            _context->PSSetShaderResources(0, (uint)numberOfResources, resources);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnbindVertexShaderResources(in Handle<Texture>[] handles)
        {
            if (handles == null)
            {
                return;
            }
            var numberOfResources = handles.Length;
            var resources = stackalloc ID3D11ShaderResourceView*[numberOfResources];
            MemoryUtils.Init(resources, sizeof(ID3D11ShaderResourceView*) * numberOfResources);
            _context->VSSetShaderResources(0, (uint)numberOfResources, resources);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShaderConstantBuffer(in Handle<ResourceBuffer> handle, uint slot = 0u)
        {
            Debug.Assert(GraphicsDevice.BufferManager.Access(handle).BindFlag.HasFlag(D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER));

            var buffer = GraphicsDevice.BufferManager.Access(handle).Resource;
            _context->VSSetConstantBuffers(slot, 1, &buffer);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderConstantBuffer(in Handle<ResourceBuffer> handle, uint slot = 0u)
        {
            Debug.Assert(GraphicsDevice.BufferManager.Access(handle).BindFlag.HasFlag(D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER));

            var buffer = GraphicsDevice.BufferManager.Access(handle).Resource;
            _context->PSSetConstantBuffers(slot, 1, &buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexBuffer(in Handle<ResourceBuffer> handle)
        {
            ref readonly var buffer = ref GraphicsDevice.BufferManager.Access(handle);
            Debug.Assert(buffer.BindFlag.HasFlag(D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER));
            var resource = buffer.Resource;
            var stride = buffer.Stride;
            var offset = 0u;
            _context->IASetVertexBuffers(0, 1, &resource, &stride, &offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetIndexBuffer(in Handle<ResourceBuffer> handle)
        {
            ref readonly var buffer = ref GraphicsDevice.BufferManager.Access(handle);
            Debug.Assert(buffer.BindFlag.HasFlag(D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER));
            var format = buffer.Stride == 2 ? DXGI_FORMAT.DXGI_FORMAT_R16_UINT : DXGI_FORMAT.DXGI_FORMAT_R32_UINT;
            _context->IASetIndexBuffer(buffer.Resource, format, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawIndexed(uint count, uint startIndex = 0, int vertexIndex = 0) => _context->DrawIndexed(count, startIndex, vertexIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(uint vertexCount, uint vertexStartIndex = 0u) => _context->Draw(vertexCount, vertexStartIndex);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY topology) => _context->IASetPrimitiveTopology(topology);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetViewPort(in ViewPort viewport)
        {
            fixed (D3D11_VIEWPORT* pView = &viewport.Resource)
            {
                _context->RSSetViewports(1, pView);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearDepthBuffer(in Handle<Texture> handle, float value) => _context->ClearDepthStencilView(GraphicsDevice.TextureManager.Access(handle).D3DDepthStencil, D3D11_CLEAR_FLAG.D3D11_CLEAR_DEPTH | D3D11_CLEAR_FLAG.D3D11_CLEAR_STENCIL, value, 0);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetRasterizerState(in Handle<RasterizerState> handle)
        {
            if (handle.IsValid())
            {
                _context->RSSetState(GraphicsDevice.RasterizerManager.Access(handle).State);
            }
            else
            {
                _context->RSSetState(null);
            }
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBlendState(in Handle<BlendState> handle)
        {
            if (handle.IsValid())
            {
                ref readonly var blendState = ref GraphicsDevice.BlendStateManager.Access(handle);
                fixed (float* pBlendFactor = blendState.BlendFactor)
                {
                    _context->OMSetBlendState(blendState.State, pBlendFactor, blendState.SampleMask);
                }
            }
            else
            {
                _context->OMSetBlendState(null, null, 0xffffffff);
            }
        }
    }
}
