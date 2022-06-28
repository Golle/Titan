namespace Titan.Modules;

public record struct GameStart;
public record struct GameEnd;

public class CoreModule : IModule
{
    public static void Build(App app)
    {
        app
            .AddEvent<GameStart>(1)
            .AddEvent<GameEnd>(1);
    }
}
