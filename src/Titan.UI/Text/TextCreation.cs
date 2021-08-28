using System;

namespace Titan.UI.Text
{
    public ref struct TextCreation
    {
        public ReadOnlySpan<char> InitialCharacters;
        public uint MaxCharacters;
        public bool Dynamic;
    }
}
