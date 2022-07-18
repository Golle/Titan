using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core;
using Titan.ECS.Entities;

namespace Titan.ECS.Components;

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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <returns>Returns true if the component was Created</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Create(in Entity entity, in T value = default) => _vtbl->Create(_data, entity, value);
    /// <summary>
    /// tbd
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <returns>Returns true if the component was Created</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool CreateOrReplace(in Entity entity, in T value) => _vtbl->CreateOrReplace(_data, entity, value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Contains(in Entity entity) => _vtbl->Contains(_data, entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Destroy(in Entity entity) => _vtbl->Destroy(_data, entity);
}
