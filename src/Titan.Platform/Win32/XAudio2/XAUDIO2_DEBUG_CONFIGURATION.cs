namespace Titan.Platform.Win32.XAudio2;

public struct XAUDIO2_DEBUG_CONFIGURATION
{
    public uint TraceMask;                   // Bitmap of enabled debug message types.
    public uint BreakMask;                   // Message types that will break into the debugger.
    public /*BOOL*/int LogThreadID;          // Whether to log the thread ID with each message.
    public /*BOOL*/int LogFileline;          // Whether to log the source file and line number.
    public /*BOOL*/int LogFunctionName;      // Whether to log the function name.
    public /*BOOL*/int LogTiming;            // Whether to log message timestamps.
}
