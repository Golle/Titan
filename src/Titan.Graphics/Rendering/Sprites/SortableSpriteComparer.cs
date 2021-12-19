using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Titan.Graphics.Rendering.Sprites
{
    internal class SortableSpriteComparer : IComparer<SortableRenderable>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(SortableRenderable x, SortableRenderable y) => x.Key.CompareTo(y.Key);
    }
}
