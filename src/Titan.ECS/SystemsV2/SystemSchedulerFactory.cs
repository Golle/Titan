using System.Linq;
using Titan.Core.Memory;

namespace Titan.ECS.SystemsV2;

public static unsafe class SystemSchedulerFactory
{
    public static void TestTHis(IApp app, in PermanentMemory mem, WorldConfig config) => CreateSystemsGraph(mem, config.SystemDescriptors.ToArray(), app);

    internal static void CreateSystemsGraph(in PermanentMemory memory, in SystemDescriptor[] descriptors, IApp app)
    {
        foreach (var desc in descriptors)
        {
            SystemDependencyState state = default;
            var node = SystemNode.CreateAndInit(memory, desc, new SystemsInitializer(app, &state));
        }
    }
}
