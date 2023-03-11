namespace Titan.Setup.Configs;

public record WindowConfig(uint Width = 1024, uint Height = 768) : IConfiguration, IDefault<WindowConfig>
{
    public string Title { get; init; } = "n/a";
    public bool Windowed { get; init; } = true;
    public bool AlwaysOnTop { get; init; } = false;
    public bool Resizable { get; init; } = true;
    public static WindowConfig Default => new();
}
