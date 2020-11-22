using System;
using System.Runtime.CompilerServices;

namespace Titan.Core.Logging
{
    public class ConsoleLogger : ILog
    {
        private readonly ILogFormatter _logFormatter;

        public ConsoleLogger(ILogFormatter logFormatter)
        {
            _logFormatter = logFormatter;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Trace(string message) => StdOut(_logFormatter.Format(LogLevel.Trace, message));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Trace(string format, params object[] values) => StdOut(_logFormatter.Format(LogLevel.Trace, format, values));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Debug(string message) => StdOut(_logFormatter.Format(LogLevel.Debug, message));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Debug(string format, params object[] values) => StdOut(_logFormatter.Format(LogLevel.Debug, format, values));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Error(string message) => StdErr(_logFormatter.Format(LogLevel.Error, message));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Error(string format, params object[] values) => StdErr(_logFormatter.Format(LogLevel.Error, format, values));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Warning(string message) => StdErr(_logFormatter.Format(LogLevel.Warning, message));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Warning(string format, params object[] values) => StdErr(_logFormatter.Format(LogLevel.Warning, format, values));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void StdOut(string message) => Console.WriteLine(message);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void StdErr(string message) => Console.Error.WriteLine(message);
    }
}
