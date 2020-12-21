using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Graphics.Resources
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
    }
}
