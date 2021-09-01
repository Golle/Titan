using Titan.Assets;
using Titan.ECS.Entities;
using Titan.Graphics;
using Titan.UI.Components;

namespace Titan.UI
{
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
                    Margins = Sprite.Margins,
                    Color = Sprite.Color
                }));
                entity.AddComponent(new InteractableComponent
                {
                    Id = Identifier
                });
            }
            Text?.OnCreate(manager, entity.CreateChildEntity());
        }
    }
}
