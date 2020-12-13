using Titan.Graphics.Resources;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Textures
{
    internal class TextureLoader : ITextureLoader
    {
        private readonly IImagingFactory _imagingFactory;
        private readonly ITextureManager _textureManager;
        private readonly IShaderResourceViewManager _shaderResourceViewManager;

        public TextureLoader(IImagingFactory imagingFactory, ITextureManager textureManager, IShaderResourceViewManager shaderResourceViewManager)
        {
            _imagingFactory = imagingFactory;
            _textureManager = textureManager;
            _shaderResourceViewManager = shaderResourceViewManager;
        }

        public unsafe Texture LoadTexture(string filename)
        {
            using var image = _imagingFactory.LoadImageFromFile(filename);
            var textureHandle = _textureManager.CreateTexture(image.Width, image.Height, image.Format, image.GetBuffer(), image.Stride, D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE);
            
            ref readonly var texture = ref _textureManager[textureHandle];
            var shaderResourceHandle = _shaderResourceViewManager.Create(texture.Resource, texture.Format);
            return new Texture(textureHandle, shaderResourceHandle);
        }
    }
}
