using System.Threading;

namespace Titan.ECS.TheNew;

internal readonly struct IdGenerator<T>
{
    private static volatile uint _id = 0;
    public static uint Next() => Interlocked.Increment(ref _id);
}
