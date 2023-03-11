using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Titan.Core;


/// <summary>
/// A wrapper on top of GCHandle but with a reference counter (debug mode). We can use this when we have managed resources inside the systems
/// </summary>
/// <typeparam name="T">The managed resource</typeparam>
public struct ObjectHandle<T> where T : class
{
    private GCHandle _handle;
    public T Value
    {
        readonly get
        {
            Debug.Assert(_handle.IsAllocated);
            return (T)_handle.Target!;
        }
        set
        {
            if (!_handle.IsAllocated)
            {
                _handle = GCHandle.Alloc(value);
                GCHandleCounter.GCHandleAlloced();
            }
            else
            {
                _handle.Target = value;
            }
        }
    }

    public ObjectHandle(T obj)
    {
        Value = obj;
    }

    public void Release()
    {
        if (_handle.IsAllocated)
        {
            _handle.Free();
            GCHandleCounter.GCHandleFreed();
        }

        _handle = default;
    }

    public static implicit operator T(ObjectHandle<T> handle) => handle.Value;
}
