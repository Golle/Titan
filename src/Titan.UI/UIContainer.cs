using System.Collections.Generic;

namespace Titan.UI
{
    public class UIContainer : UIComponent
    {
        private readonly List<UIComponent> _children = new();
        public IReadOnlyList<UIComponent> Children => _children;

        public void AddButton(UIButton button)
        {
            _children.Add(button);
        }
        public void Add(UIComponent component)
        {
            _children.Add(component);
        }
    }
}
