using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Titan.Assets;
using Titan.Core.Logging;
using Titan.ECS.Entities;
using Titan.ECS.Worlds;
using Titan.Graphics.Loaders.Atlas;
using Titan.UI.Common;
using Titan.UI.Components;
using Titan.UI.Text;

namespace Titan.UI
{
    public class Sprite
    {
        public string Identifier { get; set; }
        public int Index { get; set; }
        public Margins Margins { get; set; }
    }


    public class UIPanel : UIContainer
    {
        public Sprite Background { get; set; }
        internal override void OnCreate(UIManager manager, in Entity entity)
        {
            base.OnCreate(manager, entity);
            if (Background != null)
            {
                entity.AddComponent(new AssetComponent<SpriteComponent>(Background.Identifier, new SpriteComponent
                {
                    TextureIndex = (byte)Background.Index,
                    Margins = Background.Margins
                }));
            }
        }
    }



    public class UIText : UIComponent
    {
        public string Text { get; set; }
        public string Font { get; set; }
        public int LineHeight { get; set; }
        public HorizontalOverflow HorizontalOverflow{ get; set; }
        public VerticalOverflow VerticalOverflow { get; set; }
        internal override void OnCreate(UIManager manager, in Entity entity)
        {
            base.OnCreate(manager, entity);
            entity.AddComponent(new AssetComponent<TextComponent>(Font, new TextComponent
            {
                LineHeight =  (ushort)LineHeight,
                HorizontalOverflow = HorizontalOverflow,
                VerticalOverflow = VerticalOverflow,
                Handle = manager.TextManager.Create(new TextCreation
                {
                    MaxCharacters = 100,
                    InitialCharacters = Text,
                    Dynamic = false
                })
            }));
        }
    }
    public class UIButton : UIComponent
    {
        public Sprite Sprite { get; set; }
        public Sprite OnHover { get; set; }
        public UIText Text { get; set; }
        


        internal override unsafe void OnCreate(UIManager manager, in Entity entity)
        {
            base.OnCreate(manager, entity);
            if (Sprite != null)
            {
                entity.AddComponent(new AssetComponent<SpriteComponent>(Sprite.Identifier, new SpriteComponent
                {
                    TextureIndex = (byte)Sprite.Index,
                    Margins = Sprite.Margins
                }));
                entity.AddComponent(new InteractableComponent());
            }
            Text?.OnCreate(manager, entity.CreateChildEntity());
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
        public void Add(UIComponent component)
        {
            _children.Add(component);
        }
    }

    public abstract class UIComponent
    {
        private Entity _entity;
        public ref readonly Entity Entity => ref _entity;
        public Size Size { get; set; }
        public Vector2 Offset { get; set; }
        public int ZIndex { get; set; }
        public AnchorPoint AnchorPoint { get; set; }
        public Vector2 Pivot { get; set; } = new (0.5f, 0.5f); // default to center
        internal virtual void OnCreate(UIManager manager, in Entity entity)
        {
            //Debug.Assert(Pivot.X is >= 0 and <= 1.0f && Pivot.Y is >= 0 and <= 1.0f, "Pivot must be in the range of <0.0f 0.0f> to <1.0f 1.0f>");
            _entity = entity;
            entity.AddComponent(new RectTransform
            {
                Size = Size,
                Offset = Offset,
                ZIndex = ZIndex,
                AnchorPoint = AnchorPoint,
                Pivot = Vector2.Clamp(Pivot, Vector2.Zero, Vector2.One)
            });
        }
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
