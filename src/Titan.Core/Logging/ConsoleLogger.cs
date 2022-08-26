using System;
using System.Globalization;

namespace Titan.Core.Logging;

public class ConsoleLogger : ILogger
{
    public void OnStart()
    {
        // noop
    }

    public void OnMessage(in LogMessage message)
    {
        Console.ResetColor();

        //NOTE(Jens): This is a Raw log call, don't set any colors just print it
        if (message.Level == 0)
        {
            Console.WriteLine(message.Message);
            return;
        }

        var color = message.Level switch
        {
            LogLevel.Debug or LogLevel.Trace => ConsoleColor.Cyan,
            LogLevel.Info => ConsoleColor.DarkCyan,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            _ => throw new ArgumentOutOfRangeException()
        };
        Console.ResetColor();
        Console.Write("{0} [", DateTimeNow());
        Console.ForegroundColor = color;
        Console.Write(message.Level);
        if (message.Scope != null)
        {
            Console.ResetColor();
            Console.Write("][");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(message.Scope);
        }
        Console.ResetColor();
        Console.WriteLine("] - {0}", message.Message);

        static string DateTimeNow() => DateTime.Now.ToString("HH:mm:ss.fff", new DateTimeFormatInfo());
    }

    public void OnShutdown()
    {
        //noop
    }
}
