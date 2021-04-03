using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.ECS.Assets
{
    public readonly struct Asset<T> where T : unmanaged
    {
        public readonly string Identifier;
        public Asset(string identifier) => Identifier = identifier;
    }

    [SkipLocalsInit]
    public unsafe struct UnmanagedAsset<T>
    {
        private const int MaxAssetLength = 256;

        private fixed char _chars[256];
        private readonly short _length;

        public UnmanagedAsset(string identifer)
        {
            Debug.Assert(identifer.Length <= MaxAssetLength);
            fixed (char* pChars = _chars)
            {
                identifer.ToCharArray().CopyTo(new Span<char>(pChars, 256));
            }
            _length = (short) identifer.Length;
        }

        public override string ToString()
        {
            fixed (char* pChar = _chars)
            {
                return new(pChar, 0, _length);
            }
        }
    }
}
