namespace Titan.Tools.Packager;

public record PipelineContext
{
    public bool Failed { get; init; }
    public string? Reason { get; init; }
    public string? OutputPath { get; init; }
    public string?[] ManifestPaths { get; init; } = Array.Empty<string?>();
    public string? Namespace { get; init; }
    public string? GeneratedCodePath { get; init; }
    public string? LibraryPath { get; init; }
    public int ManifestStartId { get; init; } = 1; // 0 is reserved for Engine Assets, set default to 1
}
