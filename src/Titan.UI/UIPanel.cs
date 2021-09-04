using Titan.Assets;
using Titan.ECS.Entities;
using Titan.UI.Components;

namespace Titan.UI
{
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
                    Margins = Background.Margins,
                    Color = Background.Color
                }));
            }
        }
    }
}
