using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Assets;
using Titan.ECS.Entities;
using Titan.ECS.Worlds;
using Titan.Graphics.Loaders.Atlas;
using Titan.UI.Common;
using Titan.UI.Components;

namespace Titan.UI
{
    public class Sprite
    {
        public string Identifier { get; set; }
        public int Index { get; set; }
        public Margins Margins { get; set; }
    }


    public class UIButton : UIComponent
    {
        public Sprite Sprite { get; set; }
        public Sprite OnHover { get; set; }
        internal override unsafe void OnCreate(in Entity entity)
        {
            base.OnCreate(entity);
            if (Sprite != null)
            {
                entity.AddComponent(new AssetComponent<SpriteComponent>(Sprite.Identifier, new SpriteComponent
                {
                    TextureIndex = (byte)Sprite.Index,
                    Margins = Sprite.Margins
                }));
                entity.AddComponent(new InteractableComponent());
            }
        }
    }

    public class UIContainer : UIComponent
    {
        private readonly List<UIComponent> _children = new();
        public IReadOnlyList<UIComponent> Children => _children;

        public void AddButton(UIButton button)
        {
            _children.Add(button);
        }
    }

    public abstract class UIComponent
    {

        private Entity _entity;
        public ref readonly Entity Entity => ref _entity;

        public Size Size { get; set; }
        public Vector2 Offset { get; set; }
        public int ZIndex { get; set; }
        public bool IsDirty { get; set; }

        internal virtual void OnCreate(in Entity entity)
        {
            _entity = entity;
            _entity.AddComponent(new RectTransform
            {
                Size = Size,
                Offset = Offset,
                ZIndex = ZIndex
            });
        }
    }


    public class UIManager
    {
        private readonly World _world;
        private readonly List<UIComponent> _components = new();
        
        private readonly Entity _baseEntity;

        public UIManager(World world)
        {
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
            component.OnCreate(parent.CreateChildEntity());

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
