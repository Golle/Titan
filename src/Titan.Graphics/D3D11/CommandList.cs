using System;
using System.Runtime.CompilerServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11
{

    // TODO: using a IDisposable(try/final block) in a frame might be bad for performance, consider using an inline Release method instead.
    public readonly struct CommandList : IDisposable
    {
        internal readonly unsafe ID3D11CommandList* Ptr;
        public unsafe CommandList(ID3D11CommandList* commandList)
        {
            Ptr = commandList;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Dispose() => Ptr->Release();
    }
}
