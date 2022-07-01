using System.Collections.Generic;
using Titan.Core.App;

namespace Titan.ECS.SystemsV2;

public class SystemDescriptorCollection
{
    private readonly List<SystemDescriptor> _descriptors = new();
    public void AddSystem<T>(Stage stage) where T : unmanaged, IStructSystem<T>
    {
        _descriptors.Add(SystemDescriptor.Create<T>(stage));
    }


}
