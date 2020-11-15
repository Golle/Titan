using System.Diagnostics;
using System.Runtime.CompilerServices;

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
    }
}
