using Titan;
using Titan.Graphics.Windows;

Engine.Start(new SandboxGame());

internal class SandboxGame : Game
{
    public override EngineConfiguration ConfigureEngine(EngineConfiguration config) => config;

    public override WindowConfiguration ConfigureWindow(WindowConfiguration config) =>
        config with
        {
            Height = 300,
            Width = 400,
            RawInput = false,
            Resizable = false,
            Windowed = true
        };
}
