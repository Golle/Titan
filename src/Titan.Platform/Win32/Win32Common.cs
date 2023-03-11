using System.Runtime.CompilerServices;

namespace Titan.Platform.Win32;

public static class Win32Common
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FAILED(in HRESULT result) => result.Value < 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SUCCEEDED(in HRESULT result) => result.Value >= 0;

    public const uint INFINITE = uint.MaxValue;

    public static readonly HRESULT ERROR_NOT_FOUND = 0x80070490;
}
