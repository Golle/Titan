namespace Titan.Core.Logging
{
    public interface ILog
    {
        void Trace(string message);
        void Trace(string format, params object[] values);
        void Debug(string message);
        void Debug(string format, params object[] values);
        void Error(string message);
        void Error(string format, params object[] values);
        void Warning(string message);
        void Warning(string format, params object[] values);
    }
}
