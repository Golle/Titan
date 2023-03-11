using System.Diagnostics;
using Titan.Resources;

namespace Titan.Systems;
internal class SystemsRegistry
{
    private readonly List<SystemDescriptor> _descriptors = new();
    public void AddSystem<T>(SystemStage stage, RunCriteria criteria, int priority) where T : unmanaged, ISystem
    {
        Debug.Assert(_descriptors.Any(d => d.Id == ResourceId.Id<T>()) == false, $"System of type {typeof(T).Name} has already been registered.");
        _descriptors.Add(SystemDescriptor.Create<T>(stage, criteria, priority));
    }
    public SystemDescriptor[] GetDescriptors() => _descriptors.ToArray();
}
