using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Core
{
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    public readonly struct Handle<T>
    {
        public readonly int Value; // TODO: test difference between using pack 4 vs pack 8 (8 is default, so this struct will take up 8 bytes of memory even though it's just an int.
        public Handle(int value)
        {
            Value = value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(in Handle<T> handle) => handle.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Handle<T>(in int handle) => new(handle);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid() => Value > 0;

        public static Handle<T> Null = 0;
    }
}
