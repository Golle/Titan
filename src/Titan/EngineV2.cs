using System.Threading;
using Titan.Core.Logging;
using Titan.Graphics.Windows;

namespace Titan;

public class EngineV2
{
    private readonly Game _app;

    public static void Start(Game game)
    {
        try
        {
            new EngineV2(game)
                .Start();
        }
        catch
        {
            // ignored
        }
        finally
        {
            Logger.Shutdown();
        }

    }
    private EngineV2(Game app)
    {
        _app = app;
    }

    private void Start()
    {
        static void Info(string message) => Logger.Info<EngineV2>(message);
        static void Trace(string message) => Logger.Trace<EngineV2>(message);

        Logger.Start();


        Info("Starting the engine!");

        var windowConfig = _app.ConfigureWindow(new WindowConfiguration("n/a", 800, 600, true));

        using var window = WindowV2.Create(windowConfig);

        var active = true;
        var thread = new Thread(() =>
        {
            Logger.Error("LETS GO!");
            while (active)
            {
                
                //Logger.Warning("Render");
                Thread.Sleep(200);
            }
            
            Logger.Error("lol no!");
        });

        thread.Start();

        while (window.Update())
        {

            
        }
        active = false;
        thread.Join();
    }
}
