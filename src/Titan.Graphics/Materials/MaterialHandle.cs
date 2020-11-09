using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Graphics.Materials
{
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    public readonly partial struct MaterialHandle
    {
        public readonly int Value;
        public MaterialHandle(int value)
        {
            Value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(in MaterialHandle handle) => handle.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator MaterialHandle(in int handle) => new MaterialHandle(handle);
    }
}
