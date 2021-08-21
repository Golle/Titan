using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics.Loaders.Fonts;

namespace Titan.UI.Text
{

    public struct CharacterPositions
    {
        public Vector2 BottomLeft;
        public Vector2 TopRight;
    }
    public struct TextBlock
    {
        public Handle<Font> Font;
        public MemoryChunk<CharacterPositions> Positions;
        public MemoryChunk<char> Characters;
        public ushort CharacterCount;
        public ushort VisibleChars;
        public ushort LineHeight;
        public bool Dynamic;
        public bool IsDirty;
    }

    public unsafe class TextManager : IDisposable
    {
        public FontManager FontManager { get; }

        private ResourcePool<TextBlock> _resources;
        public TextManager(uint maxTextBlocks, FontManager fontManager)
        {
            FontManager = fontManager;
            _resources.Init(maxTextBlocks);
        }

        public Handle<TextBlock> Create(in TextCreation args)
        {
            var handle = _resources.CreateResource();
            var textBlock = _resources.GetResourcePointer(handle);
            // TODO: allocate these in the same block
            textBlock->Positions = MemoryUtils.AllocateBlock<CharacterPositions>(args.MaxCharacters);
            textBlock->Characters = MemoryUtils.AllocateBlock<char>(args.MaxCharacters);
            textBlock->CharacterCount = (ushort)args.InitialCharacters.Length;
            textBlock->VisibleChars = 0;
            textBlock->Dynamic = args.Dynamic;
            textBlock->LineHeight = args.LineHeight;
            textBlock->IsDirty = true;

            fixed (char* pCharacters = args.InitialCharacters)
            {
                Buffer.MemoryCopy(pCharacters, textBlock->Characters.AsPointer(), textBlock->Characters.Size, args.InitialCharacters.Length * sizeof(char));
            }

            return handle;
        }


        public void ReCalculate(Handle<TextBlock> handle)
        {
            InitTextBlock(_resources.GetResourcePointer(handle));
        }

        public void SetFont(in Handle<TextBlock> handle, in Handle<Font> font)
        {
            var textBlock = _resources.GetResourcePointer(handle);
            textBlock->Font = font;
            textBlock->IsDirty = true;
        }

        public void SetText(Handle<TextBlock> handle, in ReadOnlySpan<char> text)
        {
            var textBlock = _resources.GetResourcePointer(handle);
            if (textBlock->Dynamic)
            {
                // TODO: add logic to re-allocate the array that holds the font
                throw new NotImplementedException("Dyanmic has not been implemented yet");
            }
            else
            {

                //Buffer.MemoryCopy(text, textBlock->Characters, textBlock->Characters.Size, text.Length*sizeof(char));
            }
        }

        private void InitTextBlock(TextBlock* textBlock)
        {
            ref readonly var font = ref FontManager.Access(textBlock->Font);
            var maxCharacters = textBlock->CharacterCount;
            var xOffset = 0;
            var lineHeight = textBlock->LineHeight;

            // TODO: add support for max X (box size and overflow functions)
            for (var i = 0; i < maxCharacters; ++i)
            {
                var character = textBlock->Characters[i];
                ref var characterBlock = ref textBlock->Positions[i];
                ref readonly var glyph = ref font.Get(character);
                characterBlock.BottomLeft = new Vector2(xOffset + glyph.XOffset, lineHeight - glyph.YOffset - glyph.Height);
                characterBlock.TopRight = new Vector2(xOffset + glyph.XAdvance, lineHeight - glyph.YOffset);
                xOffset += glyph.XAdvance;
            }

            textBlock->VisibleChars = maxCharacters;
            textBlock->IsDirty = false;
        }

        public void Dispose()
        {
            foreach (var handle in _resources.EnumerateUsedResources())
            {
                var resource = _resources.GetResourcePointer(handle);
                resource->Positions.Free();
                resource->Characters.Free();
            }
            _resources.Terminate();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly TextBlock Access(in Handle<TextBlock> handle) => ref _resources.GetResourceReference(handle);
    }


    public ref struct TextCreation
    {
        public ReadOnlySpan<char> InitialCharacters;
        public ushort LineHeight;
        public uint MaxCharacters;
        public bool Dynamic;
    }
}
