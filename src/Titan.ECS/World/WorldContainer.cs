using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.ECS.Entities;

namespace Titan.ECS.World
{
    internal class WorldContainer
    {
        private static readonly World[] Worlds = new World[100];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Remove(World world)
        {
            Debug.Assert(Worlds[world.Id] != null, "Trying to remove a world that hasn't been added.");
            Worlds[world.Id] = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(World world)
        {
            Debug.Assert(Worlds[world.Id] == null, "Trying to add a world that has already been added.");
            Worlds[world.Id] = world;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity CreateEntity(uint worldId)
        {
            Debug.Assert(Worlds[worldId] != null, "World does not exist");
            return Worlds[worldId].CreateEntity();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestroyEntity(in Entity entity)
        {
            Debug.Assert(Worlds[entity.WorldId] != null, "World does not exist");
            Worlds[entity.WorldId].DestroyEntity(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AttachEntity(in Entity parent, in Entity child)
        {
            Debug.Assert(parent.WorldId == child.WorldId, "Trying to attach a child to an entity of a different world");
            Debug.Assert(Worlds[parent.WorldId] != null, "World does not exist");
            Worlds[parent.WorldId].Attach(parent, child);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DetachEntity(in Entity child)
        {
            Debug.Assert(Worlds[child.WorldId] != null, "World does not exist");
            Worlds[child.WorldId].Detach(child);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddComponent<T>(in Entity entity) where T : unmanaged
        {
            Debug.Assert(Worlds[entity.WorldId] != null, "World does not exist");
            Worlds[entity.WorldId].AddComponent<T>(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddComponent<T>(in Entity entity, in T value) where T : unmanaged
        {
            Debug.Assert(Worlds[entity.WorldId] != null, "World does not exist");
            Worlds[entity.WorldId].AddComponent(entity, value);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveComponent<T>(in Entity entity) where T : unmanaged
        {
            Debug.Assert(Worlds[entity.WorldId] != null, "World does not exist");
            Worlds[entity.WorldId].RemoveComponent<T>(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddManagedComponent<T>(in Entity entity, in T initialValue) where T : struct
        {
            Debug.Assert(Worlds[entity.WorldId] != null, "World does not exist");
            Worlds[entity.WorldId].AddManagedComponent(entity, initialValue);
        }
    }
}
