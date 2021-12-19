using Titan.Core.Memory;

namespace Titan.Graphics.Rendering.Text
{
    public struct TextBlock
    {
        public MemoryChunk<CharacterPositions> VisibleCharacters;
        public MemoryChunk<char> Characters;
        public ushort CharacterCount;
    }
}
