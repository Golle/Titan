using System;
using Titan.Core.Images;
using Titan.Graphics.D3D11;

namespace Titan.Graphics.Textures
{
    internal class BitmapTextureLoader : ITextureLoader
    {
        private readonly IGraphicsDevice _device;

        private readonly IBitmapLoader _bitmapLoader;

        public BitmapTextureLoader(IBitmapLoader bitmapLoader, IGraphicsDevice device)
        {
            _bitmapLoader = bitmapLoader;
            _device = device;
        }


        public Texture2D LoadTexture(string filename)
        {
            var bitmap = _bitmapLoader.Load(filename);
            throw new NotImplementedException("NOt ready yet.");




        }
    }
}
