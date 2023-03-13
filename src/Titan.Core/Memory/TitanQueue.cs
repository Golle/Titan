using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Core.Memory;

public unsafe struct TitanQueue<T> where T : unmanaged
{
    //NOTE(Jens): If we enforce power of 2 sizes we can use a mask instead of modulo. It's faster but might not be needed.
    private readonly TitanArray<T> _queue;
    private volatile int _head;
    private int _tail;
    private readonly int _max;


    public TitanQueue(T* mem, uint count)
    {
        _queue = new TitanArray<T>(mem, count);
        _head = 0;
        _max = (int)count;
        Debug.Fail("This queue has not been properly implemented yet. ");
    }

    //NOTE(Jens): This fails when the queue gets full. can we do it in some other weay?
    public bool HasItems => _head != _tail;

    public ref readonly T Peek()
    {
        Debug.Assert(HasItems);
        return ref _queue[_tail];
    }

    public bool TryPeek(out T value)
    {
        Unsafe.SkipInit(out value);

        if (!HasItems)
        {
            return false;
        }

        value = Peek();
        return true;
    }

    public T Pop()
    {
        Debug.Assert(HasItems);
        var index = _tail;
        _tail = (_tail + 1) % _max;
        return _queue[index];
    }

    public bool TryPop(out T value)
    {
        Unsafe.SkipInit(out value);
        if (!HasItems)
        {
            return false;
        }
        value = Pop();
        return true;
    }

    public void Push(in T value)
    {
        var index = GetNextIndex();
        _queue[index] = value;
    }

    private int GetNextIndex()
    {
        int initialValue, newValue;
        do
        {
            initialValue = _head;
            newValue = (initialValue + 1) % _max;
        } while (Interlocked.CompareExchange(ref _head, newValue, initialValue) != initialValue);

        //NOTE(Jens): we return initial value instead of the incremented value. this is to make sure we keep head and tail in sync.
        return initialValue;
    }
}
