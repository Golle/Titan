using System.Collections.Generic;
using Titan.UI.Common;

namespace Titan.UI
{
    public class UIContainer : UIElement
    {
        private List<UIElement> _children = new();

        public UIContainer(){}
        public UIContainer(in Vector2Int position, in Size size)
        {
            Size = size;
            Position = position;
        }

        public UIContainer(int x, int y, int width, int height)
        {
            Size = new Size { Height = height, Width = width };
            Position = new Vector2Int { X = x, Y = y };
        }

        public void Add(UIElement element)
        {
            _children.Add(element);
        }

        public void Remove(UIElement element)
        {
            _children.Remove(element);
        }
    }
}
