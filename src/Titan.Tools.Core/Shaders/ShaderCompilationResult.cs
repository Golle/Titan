namespace Titan.Tools.Core.Shaders;

public abstract class ShaderCompilationResult : IDisposable
{
    public string Error { get; }
    public bool Failed => !string.IsNullOrWhiteSpace(Error);
    public bool Succeeded => !Failed;
    protected ShaderCompilationResult() => Error = string.Empty;
    protected ShaderCompilationResult(string error) => Error = error;
    public abstract ReadOnlySpan<byte> GetByteCode();
    public abstract void Dispose();
}
