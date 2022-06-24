using System;
using System.Collections.Generic;
using System.Linq;
using Titan.ECS.TheNew;

namespace Titan.ECS;

public record SystemsConfiguration(Type[] Types);
public class SystemsBuilder
{
    private readonly HashSet<Type> _types = new();

    public SystemsBuilder WithSystem<T>() where T : BaseSystem
    {
        _types.Add(typeof(T));
        return this;
    }

    public SystemsConfiguration Build() => new(_types.ToArray());
}
