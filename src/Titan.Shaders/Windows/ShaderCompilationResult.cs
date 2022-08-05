using System;

namespace Titan.Shaders.Windows;

public abstract class ShaderCompilationResult : IDisposable
{
    public string Error { get; }
    public bool Failed => !string.IsNullOrWhiteSpace(Error);
    public bool Succeeded => !Failed;
    protected ShaderCompilationResult(string error = null)
    {
        Error = error;
    }
    public abstract ReadOnlySpan<byte> GetByteCode();
    public abstract void Dispose();
}
