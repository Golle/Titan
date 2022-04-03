using System.Threading;

namespace Titan.ECS.TheNew;

internal static class ResourceId<T>
{
    public static readonly uint Id = ResourceIdGenerator.Next();
}
internal static class ResourceIdGenerator
{
    private static volatile uint _id = 1;
    public static uint Next() => Interlocked.Increment(ref _id);
}
