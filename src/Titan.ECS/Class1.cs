using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.ECS.World;

namespace Titan.ECS
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class ComponentAttribute : Attribute
    {
        //TODO: what should this be used for?
    }


    [SkipLocalsInit]
    [DebuggerDisplay("Entity={Id} World={WorldId}")]
    public readonly unsafe struct Entity
    {
        public readonly uint Id;
        public readonly uint WorldId;
        internal Entity(uint id, uint worldId)
        {
            Id = id;
            WorldId = worldId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator uint(in Entity entity) => entity.Id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNull() => Id == 0u && WorldId == 0;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy() => WorldContainer.DestroyEntity(this);
    }

    public struct EntityRelationship
    {
        public Entity Child;
        public Entity Next;

    }


    public interface IRelationship 
    {
        ref readonly EntityRelationship Get(in Entity entity);
    }

    public class Relationship
    {
        public EntityRelationship[] _relationships = new EntityRelationship[10_000];
        //public Entity[] _firstChild = new Entity[10_000];

        public void Attach(in Entity parent, in Entity child)
        {
            //ref var firstChild = ref _firstChild[parent];
            ref var relationship = ref _relationships[child];
            
        }


    }
    
    
    [Component]
    public struct Transform3D
    {
        public Vector3 Position;
        public Vector3 Scale;
        public Quaternion Rotation;

        public Matrix4x4 ModelMatrix;
        public Matrix4x4 WorldMatrix;


        // TODO: dirty flag? or use a list of "changed" components?
    }

    public class BaseSystem
    {

    }


    public class Transform3DSystem
    {
        private readonly IRelationship _relationship;


        public Transform3DSystem(IRelationship relationship)
        {
            _relationship = relationship;
        }

        public void Update(ReadOnlySpan<Transform3D> transform)
        {
            
        }
    }
}
