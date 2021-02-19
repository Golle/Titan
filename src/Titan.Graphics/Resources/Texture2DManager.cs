using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Common;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;

namespace Titan.Graphics.Resources
{
    internal unsafe class Texture2DManager : ITexture2DManager
    {
        private readonly IMemoryManager _memoryManager;
        private ComPtr<ID3D11Device> _device;
        
        private Texture2D* _textures;
        private uint _maxTextures;

        private int _numberOfTextures;

        public Texture2DManager(IMemoryManager memoryManager)
        {
            _memoryManager = memoryManager;
        }

        public void Initialize(IGraphicsDevice graphicsDevice)
        {
            if (_textures != null)
            {
                throw new InvalidOperationException($"{nameof(Texture2DManager)} has already been initialized.");
            }
            _device = graphicsDevice is D3D11GraphicsDevice device ? new ComPtr<ID3D11Device>(device.Ptr) : throw new ArgumentException($"Trying to initialize a D3D11 {nameof(Texture2DManager)} with the wrong device.", nameof(graphicsDevice));
            var memory = _memoryManager.GetMemoryChunkValidated<Texture2D>("Texture");
            _textures = memory.Pointer;
            _maxTextures = memory.Count;
        }
   
        public Handle<Texture2D> CreateTexture(uint width, uint height, DXGI_FORMAT format, D3D11_BIND_FLAG bindFlag) => CreateTexture(width, height, format, null, 0, bindFlag);
        public Handle<Texture2D> CreateTexture(uint width, uint height, DXGI_FORMAT format, void* buffer, uint stride, D3D11_BIND_FLAG bindFlag = default)
        {
            var desc = new D3D11_TEXTURE2D_DESC
            {
                Height = height,
                Width = width,
                Format = format,
                BindFlags = bindFlag,
                MiscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFIED,
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
                    SysMemPitch = stride
                };
                CheckAndThrow(_device.Get()->CreateTexture2D(&desc, &data, &_textures[handle].Pointer), "CreateTexture2D");
            }
            else
            {
                CheckAndThrow(_device.Get()->CreateTexture2D(&desc, null, &_textures[handle].Pointer), "CreateTexture2D");
            }

            return handle;
        }

        public void Destroy(in Handle<Texture2D> handle)
        {
            ref var texture = ref _textures[handle];
            if (texture.Pointer != null)
            {
                texture.Pointer->Release();
                texture.Pointer = null;
            }
        }

        public ref readonly Texture2D this[in Handle<Texture2D> handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _textures[handle];
        }

        public void Dispose()
        {
            if (_textures != null)
            {
                for (var i = 0; i < _numberOfTextures; ++i)
                {
                    ref var texture = ref _textures[i];
                    if (texture.Pointer != null)
                    {
                        texture.Pointer->Release();
                        texture.Pointer = null;
                    }
                }
                _numberOfTextures = 0;
                _textures = null;
                _device.Dispose();
            }
        }
    }
}
