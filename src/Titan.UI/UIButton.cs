using Titan.ECS.Entities;
using Titan.ECS.World;

namespace Titan.UI
{
    public class UIButton : UIComponent
    {
        public Sprite Sprite { get; set; }
        public Sprite OnHover { get; set; }
        public UIText Text { get; set; }

        internal override unsafe void OnCreate(UIManager manager, World world, in Entity entity)
        {
            base.OnCreate(manager, world, entity);
            //if (Sprite != null)
            //{
            //    world.AddComponent(entity, new AssetComponent<SpriteComponent>(Sprite.Identifier, new SpriteComponent
            //    {
            //        TextureIndex = (byte)Sprite.Index,
            //        Margins = Sprite.Margins,
            //        Color = Sprite.Color
            //    }));
            //}
            //world.AddComponent(entity, new InteractableComponent
            //{
            //    Id = Identifier
            //});
            //world.AddComponent(entity, new AnimateTranslation
            //{
            //    Start = Offset,
            //    End = Offset+new Vector2(100,10),
            //    State = new AnimationState
            //    {
            //        Time = 0.5f
            //    }
            //});
            //Text?.OnCreate(manager, world, entity.CreateChildEntity());
        }
    }
}
