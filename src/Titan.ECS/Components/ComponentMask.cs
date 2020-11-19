using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core.Messaging;
using Titan.ECS.Entities;

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
    }

    internal unsafe class EntityFilter : IDisposable
    {
        private Entity* _entities;
        private int _numberOfEntities;

        private ComponentMask _mask;

        public EntityFilter(uint maxEntities)
        {
            _entities = (Entity*) Marshal.AllocHGlobal((int) (sizeof(Entity) * maxEntities));
        }


        public void OnComponentAdded(in Entity entity){}




        public ReadOnlySpan<Entity> GetEntities() => ReadOnlySpan<Entity>.Empty;

        public void Dispose()
        {
            if (_entities != null)
            {
                Marshal.FreeHGlobal((nint)_entities);
                _entities = null;
            }
        }
    }
}
