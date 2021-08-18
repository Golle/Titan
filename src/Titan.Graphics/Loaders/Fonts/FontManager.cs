using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Graphics.Loaders.Fonts
{
    public unsafe class FontManager : IDisposable
    {
        private ResourcePool<Font> _resources;
        public FontManager(uint maxFonts = 100)
        {
            _resources.Init(maxFonts);
        }

        public Handle<Font> Create(in FontCreation args)
        {
            var handle = _resources.CreateResource();
            if (!handle.IsValid())
            {
                throw new InvalidOperationException("Failed to Create Model Handle");
            }

            var font = _resources.GetResourcePointer(handle);
            var (min, max) = GetMinMax(args.Characters);

            font->Offset = min;
            font->Texture = args.Texture;
            var maxElements = (uint)(max - min + 1);
            font->Glyphs = MemoryUtils.AllocateBlock<Glyph>(maxElements); //Sparse array to support indexing

            foreach (ref readonly var character in args.Characters)
            {
                var characterId = character.Id - min;
                font->Glyphs[characterId] = new Glyph
                {
                    TopLeft = new Vector2(character.X / (float)args.Width, character.Y / (float)args.Height),
                    BottomRight = new Vector2((character.X + character.Width) / (float)args.Width, (character.Y + character.Height) / (float)args.Height),
                    XAdvance = character.XAdvance,
                    XOffset = character.XOffset,

                    YOffset = character.YOffset
                };
            }

            return handle;

            static (int min, int max) GetMinMax(ReadOnlySpan<GlyphDescriptor> characters)
            {
                var min = char.MaxValue;
                var max = char.MinValue;
                foreach (ref readonly var character in characters)
                {
                    if (min > character.Id)
                    {
                        min = character.Id;
                    }

                    if (max < character.Id)
                    {
                        max = character.Id;
                    }
                }
                return (min, max);
            }
        }

        public void Release(in Handle<Font> handle)
        {
            var font = _resources.GetResourcePointer(handle);
            font->Glyphs.Free();
            _resources.ReleaseResource(handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Font Access(in Handle<Font> handle) => ref _resources.GetResourceReference(handle);

        public void Dispose()
        {
            foreach (var resource in _resources.EnumerateUsedResources())
            {
                _resources.GetResourcePointer(resource.Value)->Glyphs.Free();
            }
            _resources.Terminate();
        }
    }
}
