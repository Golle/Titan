using System.Diagnostics;
using Titan.Core;

namespace Titan.Modules;

public enum LoggerType
{
    Console, 
    File
}

public unsafe struct LoggingConfiguration : IDefault<LoggingConfiguration>
{
    private const int MaxFilePathLength = 256;
    // NOTE(Jens): Add logger settings. Log level, file/console etc.
    public bool Enabled;
    public LoggerType Type;

    private fixed char _filePath[MaxFilePathLength];
    private int _filePathLength;
    public ReadOnlySpan<char> FilePath
    {
        get
        {
            if (_filePathLength == 0)
            {
                return ReadOnlySpan<char>.Empty;
            }
            fixed (char* pFilePath = _filePath)
            {
                return new ReadOnlySpan<char>(pFilePath, _filePathLength);
            }
        }
        set
        {
            Debug.Assert(value.Length < MaxFilePathLength);
            if (value.Length == 0)
            {
                return;
            }
            fixed (char* pFilePath = _filePath)
            {
                value.CopyTo(new Span<char>(pFilePath, value.Length));
            }
            _filePathLength = value.Length;
        }
    }

    public static LoggingConfiguration Default =>
        new()
        {
            Type = LoggerType.Console,
            Enabled = true
        };
}
