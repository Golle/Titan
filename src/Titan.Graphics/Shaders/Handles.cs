using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Graphics.Shaders
{
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    public readonly partial struct VertexShaderHandle
    {
        public readonly int Value;
        public VertexShaderHandle(int value)
        {
            Value = value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(in VertexShaderHandle handle) => handle.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator VertexShaderHandle(in int handle) => new VertexShaderHandle(handle);
    }
    
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    public readonly partial struct PixelShaderHandle
    {
        public readonly int Value;
        public PixelShaderHandle(int value)
        {
            Value = value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(in PixelShaderHandle handle) => handle.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator PixelShaderHandle(in int handle) => new PixelShaderHandle(handle);
    }
    
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    public readonly partial struct InputLayoutHandle
    {
        public readonly int Value;
        public InputLayoutHandle(int value)
        {
            Value = value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(in InputLayoutHandle handle) => handle.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator InputLayoutHandle(in int handle) => new InputLayoutHandle(handle);
    }

}
