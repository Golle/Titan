using Titan.Assets;
using Titan.ECS.Entities;
using Titan.ECS.World;
using Titan.UI.Components;

namespace Titan.UI
{
    public class UIPanel : UIContainer
    {
        public Sprite Background { get; set; }
        internal override void OnCreate(UIManager manager, World world, in Entity entity)
        {
            base.OnCreate(manager, world, entity);
            if (Background != null)
            {
            //    world.AddComponent(entity, new AssetComponent<SpriteComponent>(Background.Identifier, new SpriteComponent
            //    {
            //        TextureIndex = (byte)Background.Index,
            //        Margins = Background.Margins,
            //        Color = Background.Color
            //    }));
            }
        }
    }
}
