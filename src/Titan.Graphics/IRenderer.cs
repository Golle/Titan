using System;
using System.Runtime.CompilerServices;
using Titan.Graphics.D3D11;

namespace Titan.Graphics
{
    public interface IRenderer : IDisposable
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Render(Context context);
    }
}
