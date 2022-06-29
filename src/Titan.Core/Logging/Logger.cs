using System;
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


        private static void Log(LogLevel level, string message, string scope = null)
        {
            var result = Writer.TryWrite(new LogMessage(level, message, scope));
            System.Diagnostics.Debug.Assert(result, "Failed to write to channel.");
        }
    }
}
