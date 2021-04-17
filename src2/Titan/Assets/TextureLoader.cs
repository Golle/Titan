using System;
using System.Runtime.CompilerServices;
using Titan.Assets.Database;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Textures;
using Titan.Graphics.Images;
using Titan.Windows.D3D11;


namespace Titan.Assets
{
    public class TextureLoader : IAssetLoader
    {
        private readonly IImageLoader _imageLoader;
        public string Type => "texture";
        public TextureLoader(IImageLoader imageLoader)
        {
            _imageLoader = imageLoader;
        }

        public unsafe object OnLoad(in MemoryChunk<byte>[] buffers)
        {
            var buffer = buffers[0];
            Logger.Trace<TextureLoader>($"Load from buffer with size {buffer.Size}");
            using var image = _imageLoader.Load(buffer.AsSpan());
            var handle = GraphicsDevice.TextureManager.Create(new TextureCreation
            {
                Width = image.Width,
                Height = image.Height,
                Binding = TextureBindFlags.ShaderResource,
                DataStride = image.Stride,
                Format = image.Format,
                InitialData = new DataBlob(image.GetBuffer(), image.GetBufferSize()),
                Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE
            });

            return handle;
        }

        public void OnRelease(object asset)
        {
            var handle = Unsafe.Unbox<Handle<Texture>>(asset);
            Logger.Trace<TextureLoader>($"Release handle {handle}");
            GraphicsDevice.TextureManager.Release(handle);
        }

        public void Dispose() => _imageLoader.Dispose();
    }

}
