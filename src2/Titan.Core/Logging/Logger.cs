using System.Diagnostics;
using System.Threading.Channels;

namespace Titan.Core.Logging
{
    public static class Logger
    {
        private static readonly ChannelWriter<LogMessage> Writer = BackgroundLogger.Writer;

        public static void Start() => BackgroundLogger.Start();
        public static void Shutdown() => BackgroundLogger.Shutdown();


        [Conditional("DEBUG")]
        public static void Debug(string message) => Log(LogLevel.Debug, message);

        [Conditional("DEBUG")]
        public static void Debug<T>(string message) => Log(LogLevel.Debug, message, typeof(T).Name);

        [Conditional("TRACE")]
        public static void Trace(string message) => Log(LogLevel.Trace, message);

        [Conditional("TRACE")]
        public static void Trace<T>(string message) => Log(LogLevel.Trace, message, typeof(T).Name);


        public static void Info<T>(string message) => Log(LogLevel.Info, message, typeof(T).Name);
        public static void Info(string message) => Log(LogLevel.Info, message);

        public static void Error<T>(string message) => Log(LogLevel.Error, message, typeof(T).Name);
        public static void Error(string message) => Log(LogLevel.Error, message);

        public static void Warning<T>(string message) => Log(LogLevel.Warning, message, typeof(T).Name);
        public static void Warning(string message) => Log(LogLevel.Warning, message);


        private static void Log(LogLevel level, string message, string scope = null)
        {
            var result = Writer.TryWrite(new LogMessage(level, message, scope));
            System.Diagnostics.Debug.Assert(result, "Failed to write to channel.");
        }
    }
}
