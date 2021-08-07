using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.UI.Rendering
{
    [SkipLocalsInit]
    [DebuggerDisplay("{Key}")]
    internal readonly unsafe struct SortableRenderable
    {
        public readonly long Key;
        public readonly QueuedRenderable* Renderable;

#if DEBUG
        public readonly int ZIndex;
        public readonly int TextureHandle;
        public SortableRenderable(int zIndex, int textureHandle, QueuedRenderable* renderable)
        {
            Key = (long)zIndex << 32 | (long)textureHandle;
            Renderable = renderable;
            ZIndex = zIndex;
            TextureHandle = textureHandle;
        }
#else
        public SortableRenderable(int zIndex, int textureHandle, QueuedRenderable* renderable)
        {
            Key = (long)zIndex << 32 | (long)textureHandle;
            Renderable = renderable;
        }
#endif
    }
}
