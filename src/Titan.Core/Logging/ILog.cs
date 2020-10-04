namespace Titan.Core.Logging
{
    public interface ILog
    {
        void Debug(string message);
        void Debug(string format, params object[] values);
        void Error(string message);
        void Error(string format, params object[] values);
    }
}
