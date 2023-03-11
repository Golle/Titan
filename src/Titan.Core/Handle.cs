using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Core;

[DebuggerDisplay("Handle<{typeof(T).Name,nq}> {Value}")]
public readonly struct Handle<T> where T : unmanaged
{
    public static readonly Handle<T> Invalid = 0u;
    public readonly uint Value;
    public Handle(uint value) => Value = value;
    public Handle(int value) => Value = unchecked((uint)value);
    public bool IsValid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Value != 0;
    }

    public bool IsInvalid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Value == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator uint(Handle<T> handle) => handle.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Handle<T>(uint handle) => new(handle);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Handle<T>(int handle) => new(handle);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Handle<T>(Handle handle) => new(handle.Value);

#if DEBUG
    public override string ToString() => $"Handle<{typeof(T).Name}> {Value}";
#else
    public override string ToString() => Value.ToString();
#endif
}


[DebuggerDisplay("{" + nameof(Value) + "}")]
public readonly struct Handle
{
    public readonly int Value;
    public Handle(int value) => Value = value;
    public Handle() => Value = 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator int(in Handle handle) => handle.Value;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Handle(in int handle) => new(handle);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator uint(in Handle handle) => unchecked((uint)handle.Value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Handle(in uint handle) => new(unchecked((int)handle));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid() => Value != 0;
    public bool IsInvalid() => Value == 0;
    public static readonly Handle Null = 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => IsValid() ? Value.ToString() : "INVALID";
}

