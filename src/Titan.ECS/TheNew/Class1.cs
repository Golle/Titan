using System;
using Titan.ECS.Worlds;

namespace Titan.ECS.TheNew;

public class TestSystem1 : BaseSystem
{
    public TestSystem1()
    {
        GetMutableResource<uint>();
        GetReadOnlyResource<ushort>();
    }

    public override void OnUpdate()
    {
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: Start: {GetType().Name}");
        ulong a = 0;
        for (var i = 0; i < 100_000_000; ++i)
        {
            a += (ulong)(i % 31);
        }
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: Stop: {GetType().Name}");
    }
}

public class TestSystem2 : EntitySystem_
{
    public TestSystem2()
    {
        GetReadOnlyResource<uint>();
        GetReadOnly<uint>();
        GetMutable<ushort>();
    }

    public override void OnUpdate(World world)
    {
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: Start: {GetType().Name}");
        ulong a = 0;
        for (var i = 0; i < 100_000_000; ++i)
        {
            a += (ulong)(i % 31);
        }
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: Stop: {GetType().Name}");
    }
}

public class TestSystem3 : EntitySystem_
{
    public TestSystem3()
    {
        GetReadOnly<ushort>();
    }

    public override void OnUpdate(World world)
    {
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: Start: {GetType().Name}");
        ulong a = 0;
        for (var i = 0; i < 100_000_000; ++i)
        {
            a += (ulong)(i % 31);
        }
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: Stop: {GetType().Name}");
    }
}

public class TestSystem4 : BaseSystem
{
    public TestSystem4()
    {
        DependsOn<TestSystem2>();
    }

    public override void OnUpdate()
    {
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: Start: {GetType().Name}");
        ulong a = 0;
        for (var i = 0; i < 100_000_000; ++i)
        {
            a += (ulong)(i % 31);
        }
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: Stop: {GetType().Name}");
    }
}

