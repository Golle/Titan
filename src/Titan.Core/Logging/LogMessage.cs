namespace Titan.Core.Logging
{
    internal readonly struct LogMessage
    {
        public readonly string Message;
        public readonly string Scope;
        public readonly LogLevel Level;

        public LogMessage(LogLevel level, string message, string scope)
        {
            Level = level;
            Message = message;
            Scope = scope;
        }
    }
}
