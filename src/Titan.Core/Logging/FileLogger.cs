using System.Globalization;

namespace Titan.Core.Logging;

public class FileLogger : ILogger
{
    private StreamWriter _fileStream;
    private readonly string _path;

    public FileLogger(ReadOnlySpan<char> filePath)
    {
        _path = new string(filePath);

    }
    public void OnStart()
    {
        var dir = Path.GetDirectoryName(_path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir!);
        }
        _fileStream = new StreamWriter(File.OpenWrite(_path));
        _fileStream.BaseStream.Seek(0, SeekOrigin.End);
    }

    public void OnMessage(in LogMessage message)
    {
        static string DateTimeNow() => DateTime.Now.ToString("HH:mm:ss.fff", new DateTimeFormatInfo());
        
        _fileStream.WriteLine($"{DateTimeNow()} [{message.Scope}][{message.Level}] - {message.Message}");
    }

    public void OnShutdown()
    {
        _fileStream.Flush();
        _fileStream.Close();
        _fileStream = null;
    }
}
