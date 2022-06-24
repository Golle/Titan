namespace Titan.ECS.TheNew;

public class World_
{
    private readonly SystemsDispatcher_ _dispatcher;
    public string Name { get; }

    public World_(string name, SystemsDispatcher_ dispatcher)
    {
        Name = name;
        _dispatcher = dispatcher;
    }


    public void Init()
    {
        // Initialize the systems (entity systems require the world)
        // Allocate memory for pools
        _dispatcher.Init(this);
    }

    public void Update()
    {
        _dispatcher.Execute();
    }

    public void Teardown()
    {
        _dispatcher.Teardown(this);

        // Call teardown on all systems
        // Deallocate memory for pools

    }



}
