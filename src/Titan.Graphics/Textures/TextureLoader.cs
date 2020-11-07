using System.Runtime.InteropServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Textures
{
    internal class TextureLoader : ITextureLoader
    {
        private readonly IImagingFactory _imagingFactory;
        private readonly IGraphicsDevice _device;

        public TextureLoader(IImagingFactory imagingFactory, IGraphicsDevice device)
        {
            _imagingFactory = imagingFactory;
            _device = device;
        }

        public unsafe Texture LoadTexture(string filename)
        {
            var decoder = _imagingFactory.CreateDecoderFromFilename(filename);
            var size = decoder.ImageSize;
            var buffer = (byte*)Marshal.AllocHGlobal((int) size);
            try
            {
                decoder.CopyPixels(buffer, size);

                var textureHandle = _device.TextureManager.CreateTexture(decoder.Width, decoder.Height, DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM, buffer, 32, D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE);
                ref readonly var texture = ref _device.TextureManager[textureHandle];
                var shaderResourceHandle = _device.ShaderResourceViewManager.Create(texture.Resource, texture.Format);
                return new Texture(textureHandle, shaderResourceHandle);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)buffer);
            }
        }
    }
}
