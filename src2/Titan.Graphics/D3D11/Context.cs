using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Samplers;
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
        public void SetPixelShaderSampler(in Sampler sampler, uint slot = 0u)
        {
            var samplerState = sampler.SamplerState;
            _context->PSSetSamplers(slot, 1, &samplerState);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelShaderResource(in Texture texture, uint slot = 0u)
        {
            var resourceView = texture.D3DResource;
            _context->PSSetShaderResources(0, 1, &resourceView);
        }
    }
}
