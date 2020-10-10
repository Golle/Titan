using System.Diagnostics;
// ReSharper disable UnusedMember.Global

namespace Titan.Core.Logging
{
    public static class LOGGER
    {
        private static ILog _log;

        public static void InitializeLogger(ILog log)
        {
            _log = log;
        }

        [Conditional("TRACE")]
        public static void Trace(string message) => _log?.Trace(message);

        [Conditional("TRACE")]
        public static void Trace(string format, params object[] values) => _log?.Trace(format, values);

        [Conditional("DEBUG")]
        public static void Debug(string format, params object[] values) => _log?.Debug(format, values);
        [Conditional("DEBUG")]
        public static void Debug(string message) => _log?.Debug(message);
    }
}
