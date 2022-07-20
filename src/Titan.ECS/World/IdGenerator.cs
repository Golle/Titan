using System.Threading;

namespace Titan.ECS.World;

internal readonly struct IdGenerator<T>
{
    private static volatile uint _id;
    public static uint Next() => Interlocked.Increment(ref _id);
    public static uint Count => _id;
}
