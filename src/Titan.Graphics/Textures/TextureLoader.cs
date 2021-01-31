using Titan.Graphics.Images;
using Titan.Graphics.Resources;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Textures
{
    internal class TextureLoader : ITextureLoader
    {
        private readonly IImageFactory _imageFactory;
        private readonly ITexture2DManager _texture2DManager;
        private readonly IShaderResourceViewManager _shaderResourceViewManager;

        public TextureLoader(IImageFactory imageFactory, ITexture2DManager texture2DManager, IShaderResourceViewManager shaderResourceViewManager)
        {
            _imageFactory = imageFactory;
            _texture2DManager = texture2DManager;
            _shaderResourceViewManager = shaderResourceViewManager;
        }

        public unsafe Texture Load(string filename)
        {
            using var image = _imageFactory.LoadImageFromFile(filename);
            var textureHandle = _texture2DManager.CreateTexture(image.Width, image.Height, image.Format, image.GetBuffer(), image.Stride, D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE);
            
            ref readonly var texture = ref _texture2DManager[textureHandle];
            var shaderResourceHandle = _shaderResourceViewManager.Create(texture.Resource, texture.Format);
            return new Texture(shaderResourceHandle, textureHandle);
        }

        public void Release(in Texture texture)
        {
            _shaderResourceViewManager.Destroy(texture.Resource);
            _texture2DManager.Destroy(texture.Texture2D);
        }
    }
}
