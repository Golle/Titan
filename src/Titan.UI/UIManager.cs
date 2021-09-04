using System.Collections.Generic;
using Titan.ECS.Entities;
using Titan.ECS.Worlds;
using Titan.Graphics.Loaders.Atlas;
using Titan.UI.Text;

namespace Titan.UI
{

    internal struct UIComponentData
    {
        public Margins Margins;
    }


    public class UIManager
    {
        private readonly World _world;
        private readonly List<UIComponent> _components = new();
        private readonly Entity _baseEntity;

        internal TextManager TextManager { get; }
        public UIManager(World world, TextManager textManager)
        {
            TextManager = textManager;
            _world = world;
            _baseEntity = _world.CreateEntity();
        }
        
        public void Add(UIComponent component)
        {
            _components.Add(component);
            RecursiveCreate(_baseEntity, component);
        }

        private void RecursiveCreate(in Entity parent, UIComponent component)
        {
            component.OnCreate(this, parent.CreateChildEntity());
            if (component is UIContainer container)
            {
                foreach (var child in container.Children)
                {
                    RecursiveCreate(component.Entity, child);
                }
            }
        }

        public void Update()
        {
            foreach (var uiComponent in _components)
            {
                
                
            }
        }
    }
}
