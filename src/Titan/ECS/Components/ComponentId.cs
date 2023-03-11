using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.ECS.Components;

[DebuggerDisplay("{_high.ToString(\"X\")}:{_low.ToString(\"X\")}")]
[SkipLocalsInit]
public readonly struct ComponentId
{
    // TODO: If needed, add support for more than 128 components
    private readonly ulong _low, _high;
    public ComponentId(ulong low, ulong high)
    {
        _low = low;
        _high = high;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in ComponentId lh, in ComponentId rh) => lh._low == rh._low && lh._high == rh._high;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in ComponentId lh, in ComponentId rh) => lh._low != rh._low || lh._high != rh._high;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentId operator |(in ComponentId lh, in ComponentId rh) => new(lh._low | rh._low, lh._high | rh._high);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentId operator &(in ComponentId lh, in ComponentId rh) => new(lh._low & rh._low, lh._high & rh._high);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentId operator ^(in ComponentId lh, in ComponentId rh) => new(lh._low ^ rh._low, lh._high ^ rh._high);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentId operator +(in ComponentId lh, in ComponentId rh) => new(lh._low | rh._low, lh._high | rh._high);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentId operator -(in ComponentId lh, in ComponentId rh) => new((lh._low ^ rh._low) & lh._low, (lh._high ^ rh._high) & lh._high);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(in ComponentId other) => _low == other._low && _high == other._high;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in ComponentId id) => (_low & id._low) == id._low && (_high & id._high) == id._high;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSubsetOf(in ComponentId id) => (_low & id._low) == _low && (_high & id._high) == _high;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MatchesAny(in ComponentId id) => (_low & id._low) != 0ul || (_high & id._high) != 0ul;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MatchesNone(in ComponentId id) => (_low & id._low) == 0ul && (_high & id._high) == 0ul;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEmpty() => _high == 0 && _low == 0;

#if DEBUG
    public override string ToString() => $"{_high}:{_low}";
#endif
    public override bool Equals(object obj) => throw new NotSupportedException("Use == to avoid boxing");
    public override int GetHashCode() => throw new NotSupportedException("Use == to avoid boxing");
}
