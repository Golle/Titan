using System.Diagnostics;

namespace Titan.ECS.Components;

public static class ComponentId<T> where T : struct
{
    public static ComponentId Id { get; } = ComponentIdGenerator.Next();
}

// The ID generator is internal and should only be used by ComponentId<T>
internal static class ComponentIdGenerator
{
    private static readonly object Lock = new();
    private static ulong _low = 1;
    private static ulong _high = 1;
    private static int _count;
        
    public static ComponentId Next()
    {
        lock (Lock)
        {
            AssertCount();
            if (_count++ < 64)
            {
                return new ComponentId(_low <<= 1, 0ul);
            }

            return new ComponentId(0ul, _high <<= 1);
        }
    }

    [Conditional("DEBUG")]
    private static void AssertCount()
    {
        _count++;
        Debug.Assert(_count < 128, "Maximum number of components created");
    }
}
