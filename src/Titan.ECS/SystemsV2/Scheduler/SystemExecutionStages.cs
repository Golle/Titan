using System.Runtime.CompilerServices;
using Titan.Core.App;

namespace Titan.ECS.SystemsV2.Scheduler;

public readonly unsafe struct SystemExecutionStages
{
    private readonly SystemExecutionGraph* _graphs;
    private readonly uint _count;

    public SystemExecutionStages(SystemExecutionGraph* graphs, uint count)
    {
        _graphs = graphs;
        _count = count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly SystemExecutionGraph GetGraph(Stage stage) => ref _graphs[(int)stage];
}
