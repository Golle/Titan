using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.ECS.Components
{
    [DebuggerDisplay("Mask: {_mask}")]
    public readonly struct ComponentMask
    {
        private readonly ComponentId _mask;
        private ComponentMask(in ComponentId mask)
        {
            _mask = mask;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComponentMask Add(in ComponentId id) => new(_mask | id);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComponentMask Remove(in ComponentId id) => new((_mask ^ id) & _mask);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in ComponentId id) => _mask.Contains(id);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in ComponentMask mask) => (_mask & mask._mask) == mask._mask;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in ComponentMask lh, in ComponentMask rh) => lh._mask == rh._mask;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in ComponentMask lh, in ComponentMask rh) => lh._mask != rh._mask;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ComponentMask operator +(in ComponentMask mask, in ComponentId id) => mask.Add(id);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ComponentMask operator -(in ComponentMask mask, in ComponentId id) => mask.Remove(id);


        public override bool Equals(object obj) => throw new NotSupportedException("Use == to avoid boxing");
        public override int GetHashCode() => throw new NotSupportedException("Use == to avoid boxing");
    }
}
