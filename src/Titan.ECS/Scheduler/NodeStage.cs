namespace Titan.ECS.Scheduler;

public readonly unsafe struct NodeStage
{
    public readonly Node* Nodes;
    public readonly int Count;

    public NodeStage(Node* nodes, int count)
    {
        Nodes = nodes;
        Count = count;
    }
}