using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.ECS.Components;
using Titan.ECS.World;

namespace Titan.ECS.Entities
{
    [SkipLocalsInit]
    [DebuggerDisplay("Entity={Id} World={WorldId}")]
    public readonly unsafe struct Entity
    {
        public static readonly Entity Null = new();

        public readonly uint Id;
        public readonly uint WorldId;


        public Entity(uint id, uint worldId)
        {
            Id = id;
            WorldId = worldId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator uint(in Entity entity) => entity.Id;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Attach(in Entity entity) => WorldContainer.AttachEntity(this, entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Detach() => WorldContainer.DetachEntity(this);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNull() => Id == 0u;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy() => WorldContainer.DestroyEntity(this);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateChildEntity()
        {
            var entity = WorldContainer.CreateEntity(WorldId);
            Attach(entity);
            return entity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>() where T : unmanaged => WorldContainer.AddComponent<T>(this);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddComponent<T>(in T initialValue) where T : unmanaged => WorldContainer.AddComponent<T>(this, initialValue);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComponent<T>() where T : unmanaged => WorldContainer.RemoveComponent<T>(this);
    }
}
