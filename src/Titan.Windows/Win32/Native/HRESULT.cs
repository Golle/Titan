using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Windows.Win32.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct HRESULT
    {
        private readonly nint _value;
        public nint Code => _value;
        public bool Failed => _value != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Check(string className, [CallerMemberName] string function = null)
        {
            if (Failed)
            {
                throw new Win32Exception($"{className} {function} failed with code: 0x{Code.ToString("X")}");
            }
        }
    }
}
