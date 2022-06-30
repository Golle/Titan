using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.ECS.Entities;

namespace Titan.ECS.SystemsV2.Components;


// TODO(Jens): create  a ComponentSystem for each component type that will run in PreUpdate. This will handle any deletes.

[StructLayout(LayoutKind.Sequential)]
public unsafe struct Components<T> where T : unmanaged
{
    internal delegate*<void*, in Entity, ref T> GetFunc;
    internal delegate*<void*, in Entity, in T, ref T> CreateFunc;
    internal delegate*<void*, in Entity, in T, ref T> CreateOrReplaceFunc;
    internal void* Data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get(in Entity entity) => ref GetFunc(Data, entity);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Create(in Entity entity, in T value = default) => ref CreateFunc(Data, entity, value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T CreateOrReplace(in Entity entity, in T value = default) => ref CreateOrReplaceFunc(Data, entity, value);
}
