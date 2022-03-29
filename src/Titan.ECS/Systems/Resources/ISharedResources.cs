using System;

namespace Titan.ECS.Systems.Resources;

public interface ISharedResources : IDisposable
{
    unsafe T* GetMemoryForType<T>() where T : unmanaged;
}