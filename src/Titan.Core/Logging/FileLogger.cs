using System.Diagnostics;
using System.Globalization;

namespace Titan.Core.Logging;

public class FileLogger : ILogger
{
    private StreamWriter _fileStream;
    private readonly string _path;
    public FileLogger(string path)
    {
        _path = path;

    }
    public void OnStart()
    {
        Debug.Assert(_fileStream == null);
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
        Debug.Assert(_fileStream != null);
        static string DateTimeNow() => DateTime.Now.ToString("HH:mm:ss.fff", new DateTimeFormatInfo());

        _fileStream!.WriteLine($"{DateTimeNow()} [{message.Scope}][{message.Level}] - {message.Message}");
    }

    public void OnShutdown()
    {
        Debug.Assert(_fileStream != null);
        _fileStream.Flush();
        _fileStream.Close();
        _fileStream = null;
    }
}
