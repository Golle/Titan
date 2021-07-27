using System.Runtime.CompilerServices;

namespace Titan.UI.Rendering
{
    [SkipLocalsInit]
    internal readonly unsafe struct SortableRenderable
    {
        public readonly long Key;
        public readonly QueuedRenderable* Renderable;
        public SortableRenderable(int zIndex, int textureHandle, QueuedRenderable* renderable)
        {
            Key = (long)zIndex << 32 | (long)textureHandle;
            Renderable = renderable;
        }
    }
}
