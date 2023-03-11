namespace Titan.Tools.Core.CommandLine;

public record Option<T>(string Name)
{
    public string? Alias { get; init; }
    public string? Description { get; init; }
    public bool RequiresArguments { get; init; } = true;
    public Func<T, string?, T>? Callback { get; init; }
    public Func<string, bool>? Validate { get; init; }
}
