using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core;
using Titan.ECS.Entities;

namespace Titan.ECS.SystemsV2.Components;

// TODO(Jens): create  a ComponentSystem for each component type that will run in PreUpdate. This will handle any deletes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Components<T> where T : unmanaged, IComponent
{
    private readonly ComponentPoolVTable<T>* _vtbl;
    public void* _data;
    internal Components(ComponentPoolVTable<T>* vtbl, void* data)
    {
        _vtbl = vtbl;
        _data = data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ref T Get(in Entity entity) => ref _vtbl->Get(_data, entity);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ref T Create(in Entity entity, in T value = default) => ref _vtbl->Create(_data, entity, value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ref T CreateOrReplace(in Entity entity, in T value = default) => ref _vtbl->CreateOrReplace(_data, entity, value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Contains(in Entity entity) => _vtbl->Contains(_data, entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Destroy(in Entity entity) => _vtbl->Destroy(_data, entity);
}
