using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Textures;
using Titan.Graphics.Images;
using Titan.Windows.D3D11;

namespace Titan.Assets
{
    public struct TextureAsset : IAsset
    {
        private static readonly IImageLoader ImageLoader = new WICImageLoader();

        private Handle<Texture> _handle;
        public unsafe void OnLoad(in MemoryChunk<byte> buffer)
        {
            using var image = ImageLoader.Load(buffer.AsSpan());
            _handle = GraphicsDevice.TextureManager.Create(new TextureCreation
            {
                Width = image.Width,
                Height = image.Height,
                Binding = TextureBindFlags.ShaderResource,
                DataStride = image.Stride,
                Format = image.Format,
                InitialData = new DataBlob(image.GetBuffer(), image.GetBufferSize()),
                Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE
            });
        }

        public void OnRelease()
        {
            GraphicsDevice.TextureManager.Release(_handle);
        }
    }
}
