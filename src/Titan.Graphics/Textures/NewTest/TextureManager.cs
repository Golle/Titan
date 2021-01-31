using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;
using Titan.Graphics.Images;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Textures.NewTest
{
    internal class TextureFactory
    {
        private readonly IImageFactory _imageFactory;
        private readonly Texture2DFactory _texture2DFactory;
        private readonly ShaderResourceViewFactory _shaderResourceViewFactory;
        public TextureFactory(IImageFactory imageFactory, Texture2DFactory texture2DFactory, ShaderResourceViewFactory shaderResourceViewFactory)
        {
            _imageFactory = imageFactory;
            _texture2DFactory = texture2DFactory;
            _shaderResourceViewFactory = shaderResourceViewFactory;
        }

        internal unsafe Texture1 CreateFromFile(string filename)
        {
            using var image = _imageFactory.LoadImageFromFile(filename);
            var texture2D = _texture2DFactory.Create(image.Width, image.Height, image.Format, image.GetBuffer(), image.Stride, D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE);
            var shaderResource = _shaderResourceViewFactory.Create(texture2D, image.Format);
            return new Texture1(shaderResource, texture2D);
        }

        internal unsafe Texture1 CreateSingleColorTexture(in Color color)
        {
            fixed (Color* pColor = &color)
            {
                const DXGI_FORMAT format = DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT;
                var texture2D = _texture2DFactory.Create(1, 1, format, pColor, (uint) sizeof(Color), D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE);
                var shaderResource = _shaderResourceViewFactory.Create(texture2D, format);
                return new Texture1(shaderResource, texture2D);
            }
        }

        public void Release(in Texture1 texture)
        {
            texture.Texture2D.Release();
            texture.View.Release();
        }
    }
}
