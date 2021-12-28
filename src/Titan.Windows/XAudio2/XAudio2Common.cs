using System.Runtime.InteropServices;

namespace Titan.Windows.XAudio2;

public unsafe class XAudio2Common
{
    private const string DllName = "xaudio2_9";

    // REF: https://docs.microsoft.com/en-us/windows/win32/api/xaudio2/nf-xaudio2-xaudio2create
    [DllImport(DllName, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern HRESULT XAudio2Create(
        IXAudio2** ppXAudio2,
        uint Flags = 0,
        XAUDIO2_PROCESSOR XAudio2Processor = XAUDIO2_PROCESSOR.XAUDIO2_DEFAULT_PROCESSOR);
}
