using System;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Memory;

namespace Titan.UI.Text
{
    public unsafe class TextManager : IDisposable
    {
        private ResourcePool<TextBlock> _resources;
        public TextManager(uint maxTextBlocks)
        {
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

            fixed (char* pCharacters = args.InitialCharacters)
            {
                Buffer.MemoryCopy(pCharacters, textBlock->Characters.AsPointer(), textBlock->Characters.Size, args.InitialCharacters.Length * sizeof(char));
            }

            return handle;
        }
        public void SetText(Handle<TextBlock> handle, in ReadOnlySpan<char> text)
        {
            var textBlock = _resources.GetResourcePointer(handle);
            {
                // TODO: add logic to re-allocate the array that holds the font
                throw new NotImplementedException("Dyanmic has not been implemented yet");
            }
            {

                //Buffer.MemoryCopy(text, textBlock->Characters, textBlock->Characters.Size, text.Length*sizeof(char));
            }
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
        public ref TextBlock Access(in Handle<TextBlock> handle) => ref _resources.GetResourceReference(handle);
    }
}
