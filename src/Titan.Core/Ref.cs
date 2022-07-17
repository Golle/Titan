namespace Titan.Core;

/// <summary>
/// We could use this to reduce the usage of raw pointers
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly unsafe struct Ref<T> where T : unmanaged
{
    private readonly T* _ptr;
    public Ref(T* ptr)
    {
        _ptr = ptr;
    }
    public ref T AsRef() => ref *_ptr;
    public ref readonly T AsReadOnlyRef() => ref *_ptr;
    public static implicit operator Ref<T>(T* ptr) => new(ptr);
    public static implicit operator T*(in Ref<T> ptr) => ptr._ptr;
}
