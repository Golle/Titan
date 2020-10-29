using System.Runtime.InteropServices;
using Titan.Graphics.D3D11;

namespace Titan.Graphics.Textures
{
    internal class TextureLoader : ITextureLoader
    {
        private readonly IImagingFactory _imagingFactory;
        private readonly IGraphicsDevice _graphicsDevice;

        public TextureLoader(IImagingFactory imagingFactory, IGraphicsDevice graphicsDevice)
        {
            _imagingFactory = imagingFactory;
            _graphicsDevice = graphicsDevice;
        }

        public Texture LoadTexture(string filename)
        {
            var decoder = _imagingFactory.CreateDecoderFromFilename(filename);
            var size = decoder.ImageSize;
            var buffer = Marshal.AllocHGlobal((int) size);
            try
            {
                decoder.CopyPixels(buffer, size);
                var texture2D = new Texture2D(_graphicsDevice, decoder.Width, decoder.Height, buffer, size);
                var resourceView = new ShaderResourceView(_graphicsDevice, texture2D);
                return new Texture(texture2D, resourceView);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
