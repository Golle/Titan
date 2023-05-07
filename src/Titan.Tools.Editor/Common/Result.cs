namespace Titan.Tools.Editor.Common;
public record Result<T>(bool Success, string? Error = null, T? Data = default);
public record Result(bool Success, string? Error = null)
{
    public static Result Ok() => new(true);
    public static Result Fail(string error) => new(false, Error: error);
    public static Result<T> Ok<T>(T data) => new(true, Data: data);
    public static Result<T> Fail<T>(string error) => new(false, Error: error);
}

