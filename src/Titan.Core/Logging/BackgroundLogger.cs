using System.Threading;
using System.Threading.Channels;

namespace Titan.Core.Logging;

internal static class BackgroundLogger
{
    private const int MaxMessages = 1000;
    private static readonly Channel<LogMessage> LogChannel = Channel.CreateBounded<LogMessage>(MaxMessages);

    private static bool _active;
    private static readonly Thread LogThread;
    private static ILogger _logger;
    internal static ChannelWriter<LogMessage> Writer => LogChannel.Writer;

    static BackgroundLogger()
    {
        LogThread = new Thread(Run) { Priority = ThreadPriority.Lowest };
    }

    public static void Start(ILogger instance)
    {
        _logger = instance;
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
        _logger.OnStart();
        var reader = LogChannel.Reader;
        while (_active)
        {
            if (reader.TryRead(out var logMessage))
            {
                _logger.OnMessage(logMessage);
            }
            else
            {
                Thread.Sleep(10);
            }
        }

        // Flush the remaining messages
        while (reader.TryRead(out var logMessage))
        {
            _logger.OnMessage(logMessage);
        }

        _logger.OnShutdown();
    }
}
