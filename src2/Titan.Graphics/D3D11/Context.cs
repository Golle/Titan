using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Samplers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;
using Titan.Windows;
using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11
{
    public unsafe class Context
    {
        private readonly ID3D11DeviceContext* _context;
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
        public void Map<T>(in Handle<Buffer> handle, in T value) where T : unmanaged
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
        public void SetRenderTargets(in Handle<Texture>[] handles)
        {
            var numberOfViews = handles.Length;
            if (numberOfViews == 1)
            {
                var renderTarget = GraphicsDevice.TextureManager.Access(handles[0]).D3DTarget;
                _context->OMSetRenderTargets(1, &renderTarget, null);
            }
            else
            {
                var renderTargets = stackalloc ID3D11RenderTargetView*[numberOfViews];
                for (var i = 0; i < numberOfViews; ++i)
                {
                    renderTargets[i] = GraphicsDevice.TextureManager.Access(handles[i]).D3DTarget;
                }
                _context->OMSetRenderTargets((uint) numberOfViews, renderTargets, null);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderResources(in Handle<Texture>[] handles, uint startSlot = 0u)
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
        public void UnsetRenderTargets()
        {
            ID3D11RenderTargetView* renderTarget = null;
            _context->OMSetRenderTargets(1, &renderTarget, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShader(in Handle<PixelShader> handle) => _context->PSSetShader(GraphicsDevice.ShaderManager.Access(handle).Shader, null, 0u);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShader(in Handle<VertexShader> handle) => _context->VSSetShader(GraphicsDevice.ShaderManager.Access(handle).Shader, null, 0u);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsetPixelShaderResources()
        {
            ID3D11ShaderResourceView* resource = null;
            _context->PSSetShaderResources(0, 1, &resource);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsetVertexShaderResources()
        {
            ID3D11ShaderResourceView* resource = null;
            _context->VSSetShaderResources(0, 1, &resource);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertexShaderConstantBuffer(in Handle<Buffer> handle, uint slot = 0u)
        {
            Debug.Assert(GraphicsDevice.BufferManager.Access(handle).BindFlag.HasFlag(D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER));

            var buffer = GraphicsDevice.BufferManager.Access(handle).Resource;
            _context->VSSetConstantBuffers(slot, 1, &buffer);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderConstantBuffer(in Handle<Buffer> handle, uint slot = 0u)
        {
            Debug.Assert(GraphicsDevice.BufferManager.Access(handle).BindFlag.HasFlag(D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER));

            var buffer = GraphicsDevice.BufferManager.Access(handle).Resource;
            _context->VSSetConstantBuffers(slot, 1, &buffer);
        }
    }
}
