using System;
using Titan.Assets;
using Titan.Assets.Database;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;

namespace Titan.Graphics.Loaders.Fonts
{
    public unsafe class FontLoader : IAssetLoader
    {
        private readonly FontManager _manager;
        public FontLoader(FontManager manager) => _manager = manager;

        public int OnLoad(in MemoryChunk<byte>[] buffers, in ReadOnlySpan<Dependency> dependencies)
        {
            var fontBytes = buffers[0].AsPointer();

            var descriptor = (FontDescriptor*)fontBytes;
            var characters = (GlyphDescriptor*)(descriptor + 1);
            var kernings = characters + descriptor->CharactersCount;

            var textureHandle = dependencies[0].AssetHandle;
            ref readonly var texture = ref GraphicsDevice.TextureManager.Access(textureHandle);
            
            return _manager.Create(new FontCreation
            {
                LineHeight = descriptor->LineHeight,
                Base = descriptor->Base,
                Width = texture.Width,
                Height = texture.Height,
                Texture = textureHandle,
                Characters = new ReadOnlySpan<GlyphDescriptor>(characters, descriptor->CharactersCount)
            });
        }

        public void OnRelease(int handle)
        {
            _manager.Release(handle);
        }

        public void Dispose()
        {
            
        }
    }
}
