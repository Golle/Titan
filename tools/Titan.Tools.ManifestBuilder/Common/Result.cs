namespace Titan.Tools.ManifestBuilder.Common;


public record struct Result(string? Error)
{
    public bool Failed => !string.IsNullOrEmpty(Error);
    public bool Succeeded => !Failed;
    public static Result Fail(string error) => new(error);
    public static Result Success() => new(null);
}
public record struct Result<T>(T? Data, string? Error)
{
    public bool Failed => !string.IsNullOrEmpty(Error);
    public bool Succeeded => !Failed;
    public static Result<T> Fail(string error) => new(default, error);
    public static Result<T> Success(in T? data = default) => new(data, null);
}
