using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.ECS.Entities;

namespace Titan.ECS.SystemsV2.Components;


// TODO(Jens): create  a ComponentSystem for each component type that will run in PreUpdate. This will handle any deletes.

[StructLayout(LayoutKind.Sequential)]
public unsafe struct Components<T> where T : unmanaged
{
    internal ComponentPoolVTable<T> Vtbl;
    internal void* Data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get(in Entity entity) => ref Vtbl.Get(Data, entity);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Create(in Entity entity, in T value = default) => ref Vtbl.Create(Data, entity, value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T CreateOrReplace(in Entity entity, in T value = default) => ref Vtbl.CreateOrReplace(Data, entity, value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in Entity entity) => Vtbl.Contains(Data, entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy(in Entity entity) => Vtbl.Destroy(Data, entity);
}
