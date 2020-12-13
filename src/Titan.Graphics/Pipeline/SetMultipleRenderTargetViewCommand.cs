using System.Runtime.CompilerServices;
using Titan.Graphics.Resources;

namespace Titan.Graphics.Pipeline
{
    internal unsafe struct SetMultipleRenderTargetViewCommand
    {
        public uint Count;
        public fixed int Handles[4]; // TODO: increase this if we want to support a higher number of render targets
        public DepthStencilViewHandle DepthStencilView;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int i, RenderTargetViewHandle handle) => Handles[i] = handle;
    }
}
