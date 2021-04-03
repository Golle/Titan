using System;
using System.Diagnostics;
using System.Threading;
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

        [Conditional("TRACE")]
        public static void Trace(string message) => Log(LogLevel.Trace, message);

        public static void Info(string message) => Log(LogLevel.Info, message);

        public static void Error(string message) => Log(LogLevel.Error, message);

        public static void Warning(string message) => Log(LogLevel.Warning, message);


        private static void Log(LogLevel level, string message)
        {
            var result = Writer.TryWrite(new LogMessage(level, message));
            System.Diagnostics.Debug.Assert(result, "Failed to write to channel.");
        }
    }

    internal enum LogLevel
    {
        Trace = 1,
        Debug = 2,
        Info = 4,
        Warning = 8,
        Error = 16,

    }

    internal readonly struct LogMessage
    {
        public readonly string Message;
        public readonly LogLevel Level;
        public LogMessage(LogLevel level, string message)
        {
            Level = level;
            Message = message;
        }
    }

    internal static class BackgroundLogger
    {
        private const int MaxMessages = 1000;
        private static readonly Channel<LogMessage> LogChannel = Channel.CreateBounded<LogMessage>(MaxMessages);

        private static bool _active;
        private static readonly Thread LogThread;
        internal static ChannelWriter<LogMessage> Writer => LogChannel.Writer;

        static BackgroundLogger()
        {
            LogThread = new Thread(Run) { Priority = ThreadPriority.Lowest };
        }

        public static void Start()
        {
            _active = true;
            LogThread.Start();
        }

        public static void Shutdown()
        {
            _active = false;
            LogChannel.Writer.Complete();
            LogThread.Join();
        }

        private static void Run()
        {
            static string DateTimeNow() => DateTime.Now.ToString("u");
            static void WriteLine(in LogMessage logMessage)
            {
                var color = logMessage.Level switch
                {
                    LogLevel.Debug or LogLevel.Trace => ConsoleColor.Cyan,
                    LogLevel.Info => ConsoleColor.DarkCyan,
                    LogLevel.Warning => ConsoleColor.Yellow,
                    LogLevel.Error => ConsoleColor.Red,
                    _ => throw new ArgumentOutOfRangeException()
                };
                Console.Write("{0} [", DateTimeNow());
                Console.ForegroundColor = color;
                Console.Write(logMessage.Level);
                Console.ResetColor();
                Console.WriteLine("] - {0}", logMessage.Message);
            }

            var reader = LogChannel.Reader;
            while (_active)
            {
                if (reader.TryRead(out var logMessage))
                {
                    WriteLine(logMessage);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }

            // Flush the remaining messages
            while (reader.TryRead(out var logMessage))
            {
                WriteLine(logMessage);
            }
        }
    }
}
