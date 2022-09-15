using System.Runtime.CompilerServices;
using Titan.Graphics.D3D11;

namespace Titan.Graphics
{
    public abstract class Renderer : IDisposable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Render(Context context);

        public virtual void Dispose() { }
    }
}
