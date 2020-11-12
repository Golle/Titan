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
            using var image = _imagingFactory.LoadImageFromFile(filename);
            var textureHandle = _device.TextureManager.CreateTexture(image.Width, image.Height, image.Format, image.GetBuffer(), image.Stride, D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE);
            
            ref readonly var texture = ref _device.TextureManager[textureHandle];
            var shaderResourceHandle = _device.ShaderResourceViewManager.Create(texture.Resource, texture.Format);
            return new Texture(textureHandle, shaderResourceHandle);
        }
    }
}
