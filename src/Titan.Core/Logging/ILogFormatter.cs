namespace Titan.Core.Logging
{
    public interface ILogFormatter
    {
        string Format(LogLevel level, string format, params object[] values);
    }
}
