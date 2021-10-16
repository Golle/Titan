using System.ComponentModel;
using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace Titan.Windows
{
    public static class Common
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FAILED(in HRESULT result) => result.Value < 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SUCCEEDED(in HRESULT result) => result.Value >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckAndThrow(in HRESULT result, string functionName, string message = null)
        {
            if (FAILED(result))
            {
                throw new Win32Exception(result, message ?? $"Call to {functionName} failed with HRESULT {result}");
            }
        }
    }
}
