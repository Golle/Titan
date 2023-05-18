namespace Titan.Tools.Editor.Core;
public record ProcessArgs(string Command, string Arguments)
{
    public string? WorkgingDiretory { get; init; }
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(20);
    public bool CreateWindow { get; init; }
    public bool CheckForImmediateExit { get; init; } = true;
}

public record ProcessResult
{
    public bool Success { get; init; }
    public string Reason { get; init; } = string.Empty;
    public int ExitCode { get; init; }
    public string StdErr { get; init; } = string.Empty;
    public string StdOut { get; init; } = string.Empty;

}

public interface IProcessRunner
{
    Task<ProcessResult> Run(ProcessArgs args);

    Task<ProcessResult> RunNoWait(ProcessArgs args);
}
