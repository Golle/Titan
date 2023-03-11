using System.Diagnostics;

namespace Titan.Core.Logging;

public record struct LogMessage(LogLevel Level, string Message, string Scope);

public static class Logger
{
    private static BackgroundLogger _backgroundLogger;
    /// <summary>
    /// Start the logger with a Default Console Logger.
    /// </summary>
    public static IDisposable Start() => Start<ConsoleLogger>();
    public static IDisposable Start<TLogger>(uint maxMessages = 0) where TLogger : ILogger, new() => Start(new TLogger(), maxMessages);
    public static IDisposable Start(ILogger logger, uint maxMessages)
    {
        System.Diagnostics.Debug.Assert(_backgroundLogger == null);
        _backgroundLogger = new BackgroundLogger(logger, maxMessages);
        return new DisposableLogger();
    }

    public static void Shutdown()
    {
        System.Diagnostics.Debug.Assert(_backgroundLogger != null);
        _backgroundLogger.Shutdown();
    }

    [Conditional("DEBUG")]
    public static void Debug(string message) => Log(LogLevel.Debug, message);

    [Conditional("DEBUG")]
    public static void Debug<T>(string message) => Debug(message, typeof(T));

    [Conditional("DEBUG")]
    public static void Debug(string message, Type type) => Log(LogLevel.Debug, message, type.Name);

    [Conditional("TRACE")]
    public static void Trace(string message) => Log(LogLevel.Trace, message);

    [Conditional("TRACE")]
    public static void Trace<T>(string message) => Trace(message, typeof(T));

    [Conditional("TRACE")]
    public static void Trace(string message, Type type) => Log(LogLevel.Trace, message, type.Name);


    public static void Info<T>(string message) => Info(message, typeof(T));
    public static void Info(string message, Type type) => Log(LogLevel.Info, message, type.Name);
    public static void Info(string message) => Log(LogLevel.Info, message);

    public static void Error<T>(string message) => Error(message, typeof(T));
    public static void Error(string message, Type type) => Log(LogLevel.Error, message, type.Name);
    public static void Error(string message) => Log(LogLevel.Error, message);

    public static void Warning<T>(string message) => Warning(message, typeof(T));
    public static void Warning(string message, Type type) => Log(LogLevel.Warning, message, type.Name);

    public static void Warning(string message) => Log(LogLevel.Warning, message);

    public static void Raw(string message) => Log(0, message);

    private static void Log(LogLevel level, string message, string scope = null)
    {
        System.Diagnostics.Debug.Assert(_backgroundLogger != null);
        var result = _backgroundLogger.TryWrite(new LogMessage(level, message, scope));
        if (!result)
        {
            Console.Error.WriteLine("Failed to write to log channel because it's been closed. (Use Assert later.)");
        }
        //System.Diagnostics.Debug.Assert(result, "Failed to write to channel.");
    }

    private struct DisposableLogger : IDisposable
    {
        public void Dispose() => Shutdown();
    }
}
