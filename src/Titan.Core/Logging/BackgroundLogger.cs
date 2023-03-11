using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Titan.Core.Logging;

internal class BackgroundLogger : IDisposable
{
    private const int MinMessageCount = 50;
    private readonly Channel<LogMessage> _channel;
    private readonly ChannelWriter<LogMessage> _writer;
    private readonly Thread _logThread;

    private readonly ILogger _logger;
    private bool _active;

    public BackgroundLogger(ILogger logger, uint maxMessages)
    {
        _logger = logger;
        _active = true;
        _channel = Channel.CreateBounded<LogMessage>(Math.Max(MinMessageCount, (int)maxMessages));
        _writer = _channel.Writer;
        _logThread = new Thread(Run) { Priority = ThreadPriority.Lowest };
        _logThread.Start();
    }

    public void Shutdown()
    {
        if (_active)
        {
            _active = false;
            _channel.Writer.Complete();
            _logThread.Join();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryWrite(in LogMessage logMessage) 
        => _writer.TryWrite(logMessage);

    private void Run()
    {
        _logger.OnStart();
        var reader = _channel.Reader;
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

    public void Dispose() 
        => Shutdown();
}
