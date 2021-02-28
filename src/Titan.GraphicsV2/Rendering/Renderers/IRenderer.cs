using System;
using System.Runtime.CompilerServices;
using Titan.GraphicsV2.D3D11;

namespace Titan.GraphicsV2.Rendering.Renderers
{
    internal interface IRenderer : IDisposable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Render(Context context);
    }
}
