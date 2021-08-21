using System;
using Titan.Core.Memory;
using Titan.Graphics.Loaders.Fonts;

namespace Titan.UI.Rendering
{
    internal class TextBatch : IDisposable
    {
        private readonly MemoryChunk<char> _text;
        public TextBatch(uint maxCharacters, FontManager fontManager)
        {
            _text = MemoryUtils.AllocateBlock<char>(maxCharacters);
        }




        public void Dispose()
        {

            _text.Free();
        }
    }
}
