using System.Diagnostics;
using Titan.Core;

namespace Titan.ECS.Components;

public static class ComponentId<T> where T : struct, IComponent
{
    public static readonly ComponentId Id = ComponentIdGenerator.Next();
    public static readonly uint Index = IdGenerator<ComponentId>.Next() - 1; // The Component Index should start at 0
}

internal struct ComponentIdGenerator
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
