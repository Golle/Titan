using Titan.Core.Memory;

namespace Titan.UI.Text
{
    public struct TextBlock
    {
        public MemoryChunk<CharacterPositions> Positions;
        public MemoryChunk<char> Characters;
        public ushort CharacterCount;
    }
}
