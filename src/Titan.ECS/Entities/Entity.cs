using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.ECS.Worlds;

namespace Titan.ECS.Entities
{
    [SkipLocalsInit]
    [DebuggerDisplay("Entity={Id} World={WorldId}")]
    public readonly struct Entity
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
        public void Attach(in Entity entity) => World.GetWorldById(WorldId).AttachEntity(this, entity);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Detach() => World.GetWorldById(WorldId).DetachEntity(this);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNull() => Id == 0u;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy() => World.GetWorldById(WorldId).DestroyEntity(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity CreateChildEntity()
        {
            var entity = World.GetWorldById(WorldId).CreateEntity();
            Attach(entity);
            return entity;
        }

        [Conditional("DEBUG")]
        public void DebugPrintEntityRelationship() => World.GetWorldById(WorldId).Manager.DebugPrint(this);
    }
}
