using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Graphics.States
{
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    public readonly partial struct DepthStencilStateHandle
    {
        public readonly int Value;
        public DepthStencilStateHandle(int value)
        {
            Value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(in DepthStencilStateHandle handle) => handle.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DepthStencilStateHandle(in int handle) => new DepthStencilStateHandle(handle);
    }
}
