using System;
using System.Threading;
using System.Threading.Channels;

namespace Titan.Core.Logging
{
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
                if (logMessage.Scope != null)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.Write(logMessage.Scope);
                    Console.ResetColor();
                    Console.Write("][");
                }
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