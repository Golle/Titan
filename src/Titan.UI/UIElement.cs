using System.Threading;
using Titan.UI.Common;
// ReSharper disable InconsistentNaming

namespace Titan.UI
{
    public class UIElement
    {
        private static ulong _nextId = 1;
        public UIElement() => Id = Interlocked.Increment(ref _nextId);

        private Vector2Int _position;
        private Size _size;
        public ref Size Size => ref _size;
        public ref Vector2Int Position => ref _position;
        public ulong Id { get; }
        public int ZIndex { get; set; }
        public int Layer { get; set; }

        public override bool Equals(object obj) => obj is UIElement element && element.Id == Id;
        public override int GetHashCode() => Id.GetHashCode();
    }
}
