using System.Runtime.CompilerServices;
using Titan.Core.Common;

namespace Titan.Core.Logging
{
    internal class TimeLogFormatter : ILogFormatter
    {
        private readonly IDateTime _dateTime;

        public TimeLogFormatter(IDateTime dateTime)
        {
            _dateTime = dateTime;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Format(LogLevel level, string format, params object[] values)
        {
            var message = string.Format(format, values);
            return $"[{_dateTime.Now.ToLongTimeString()} | {level.ToString().ToUpper()}] {message}";
        }
    }
}
