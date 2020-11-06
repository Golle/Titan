using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Graphics.Resources
{
    // TODO: add the handles to SourceGenerators. 


    [AttributeUsage(AttributeTargets.Struct)]
    public class HandleAttribute : Attribute{}

    
    [Handle, DebuggerDisplay("{" + nameof(Value) + "}")]
    public readonly partial struct VertexBufferHandle
    {
        public readonly int Value;
        public VertexBufferHandle(int value)
        {
            Value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(in VertexBufferHandle handle) => handle.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator VertexBufferHandle(in int handle) => new VertexBufferHandle(handle);
    }

    [Handle, DebuggerDisplay("{" + nameof(Value) + "}")]
    public readonly partial struct IndexBufferHandle
    {
        public readonly int Value;
        public IndexBufferHandle(int value)
        {
            Value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(in IndexBufferHandle handle) => handle.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator IndexBufferHandle(in int handle) => new IndexBufferHandle(handle);
    }

    [Handle, DebuggerDisplay("{" + nameof(Value) + "}")]
    public readonly partial struct TextureHandle
    {
        public readonly int Value;
        public TextureHandle(int value)
        {
            Value = value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(in TextureHandle handle) => handle.Value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator TextureHandle(in int handle) => new TextureHandle(handle);
    }

    //public class Apa : ISourceGenerator

    //internal readonly struct Handle
    //{
    //    public readonly int Value;
    //    public Handle(int value)
    //    {
    //        Value = value;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static implicit operator int(in Handle handle) => handle.Value;
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static implicit operator Handle(in int handle) => new Handle(handle);
    //}
}
