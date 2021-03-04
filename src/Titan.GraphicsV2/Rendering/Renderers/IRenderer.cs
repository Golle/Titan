using System;
using System.Runtime.CompilerServices;
using Titan.GraphicsV2.D3D11;

namespace Titan.GraphicsV2.Rendering.Renderers
{
    internal interface IRenderer : IDisposable
    {
        void Init();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Render(Context context);
    }
}
