using Titan.Assets;
using Titan.ECS.Entities;
using Titan.ECS.World;
using Titan.Graphics;
using Titan.Graphics.Rendering.Text;
using Titan.UI.Components;

namespace Titan.UI
{
    public class UIText : UIComponent
    {
        public string Text { get; set; }
        public string Font { get; set; }
        public int LineHeight { get; set; }
        public int FontSize{ get; set; }
        public HorizontalOverflow HorizontalOverflow{ get; set; }
        public VerticalOverflow VerticalOverflow { get; set; }
        public TextAlign TextAlign { get; set; }
        public VerticalAlign VerticalAlign { get; set; }
        public Color Color { get; set; } = Color.Black;
        public uint MaxCharacters { get; set; }

        internal override void OnCreate(UIManager manager, World world, in Entity entity)
        {
            base.OnCreate(manager, world, entity);


            var lineheight = LineHeight > 0 ? LineHeight : FontSize;
            //world.AddComponent(entity, new AssetComponent<TextComponent>(Font, new TextComponent
            //{
            //    LineHeight =  (ushort)lineheight,
            //    HorizontalOverflow = HorizontalOverflow,
            //    VerticalOverflow = VerticalOverflow,
            //    TextAlign = TextAlign,
            //    VerticalAlign = VerticalAlign,
            //    FontSize =  (ushort)FontSize,
            //    Color = Color,
            //    Handle = manager.TextManager.Create(new TextCreation
            //    {
            //        MaxCharacters = MaxCharacters == 0 ? (uint)Text.Length : MaxCharacters,
            //        InitialCharacters = Text,
            //        Dynamic = false
            //    })
            //}));
        }
    }
}
