using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.ECS.Components
{
    [DebuggerDisplay("{_high.ToString(\"X\")}:{_low.ToString(\"X\")}")]
    public readonly struct ComponentId
    {
        // TODO: If needed, add support for more than 128 components
        private readonly ulong _low;
        private readonly ulong _high;

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

        public override string ToString() => $"{_high}:{_low}";
        public bool Equals(ComponentId other) => _low == other._low && _high == other._high;
        public override bool Equals(object obj) => obj is ComponentId other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(_low, _high);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in ComponentId id) => (_low & id._low) != 0 || (_high & id._high) != 0;
    }
}
