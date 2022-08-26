namespace Titan.Tools.ManifestBuilder.Services;


public record AppState(string? ProjectPath = null);
public interface IApplicationState
{
    string? ProjectPath { get; set; }
}

public class ApplicationState : IApplicationState
{
    public string? ProjectPath { get; set; }
}
