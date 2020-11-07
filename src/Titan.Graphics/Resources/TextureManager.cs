using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;

namespace Titan.Graphics.Resources
{
    internal unsafe class TextureManager : ITextureManager
    {
        private ComPtr<ID3D11Device> _device;
        private readonly Texture* _textures;
        private readonly uint _maxTextures;

        private int _numberOfTextures;

        public TextureManager(ID3D11Device* device, IMemoryManager memoryManager)
        {
            _device = new ComPtr<ID3D11Device>(device);
            var memory = memoryManager.GetMemoryChunk("Texture");
            Debug.Assert(memory.Stride == sizeof(Texture), "The stride of the memory chunk is not matching the expected size");
            _textures = (Texture*) memory.Pointer;
            _maxTextures = memory.Count;
        }

        public TextureHandle CreateTexture(uint width, uint height, DXGI_FORMAT format, D3D11_BIND_FLAG bindFlag) => CreateTexture(width, height, format, null, 0, bindFlag);
        public TextureHandle CreateTexture(uint width, uint height, DXGI_FORMAT format, void* buffer, uint bitsPerPixel, D3D11_BIND_FLAG bindFlag = default)
        {
            var desc = new D3D11_TEXTURE2D_DESC
            {
                Height = height,
                Width = width,
                Format = format,
                BindFlags = bindFlag,
                MiscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFICED,
                Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                ArraySize = 1,
                MipLevels = 1,
                SampleDesc = new DXGI_SAMPLE_DESC
                {
                    Count = 1,
                    Quality = 0
                }
            };
            var handle = Interlocked.Increment(ref _numberOfTextures) - 1;

            ref var texture = ref _textures[handle];
            texture.Format = desc.Format;
            texture.BindFlags = desc.BindFlags;
            texture.Width = desc.Width;
            texture.Height = desc.Height;

            if (buffer != null)
            {
                var data = new D3D11_SUBRESOURCE_DATA
                {
                    pSysMem = buffer,
                    SysMemPitch = width * bitsPerPixel / 8
                };
                CheckAndThrow(_device.Get()->CreateTexture2D(&desc, &data, &_textures[handle].Pointer), "CreateTexture2D");
            }
            else
            {
                CheckAndThrow(_device.Get()->CreateTexture2D(&desc, null, &_textures[handle].Pointer), "CreateTexture2D");
            }

            return handle;
        }

        public ref readonly Texture this[in TextureHandle handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _textures[handle];
        }

        public void Dispose()
        {
            _device.Dispose();
        }
    }
}
