namespace Titan.Core.Logging;

public interface ILogger
{
    void OnStart();
    void OnMessage(in LogMessage message);
    void OnShutdown();
}
