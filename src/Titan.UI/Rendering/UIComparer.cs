using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Titan.UI.Rendering
{
    internal class UIComparer : IComparer<SortableRenderable>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(SortableRenderable x, SortableRenderable y) => x.Key.CompareTo(y.Key);
    }
}