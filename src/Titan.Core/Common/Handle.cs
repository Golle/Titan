using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Core.Common
{
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    public readonly struct Handle<T>
    {
        public readonly int Value;
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
    }
}
