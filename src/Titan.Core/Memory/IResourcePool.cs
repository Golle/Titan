namespace Titan.Core.Memory;

internal unsafe interface IResourcePool<T> where T : unmanaged
{
    Handle<T> Alloc();
    Handle<T> SafeAlloc();
    void Free(Handle<T> handle);
    void SafeFree(Handle<T> handle);
    ref T Get(Handle<T> handle);
    T* GetPointer(Handle<T> handle);

    void Release();
}
