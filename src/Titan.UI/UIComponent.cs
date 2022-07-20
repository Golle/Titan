using System;
using System.Numerics;
using Titan.ECS.Entities;
using Titan.ECS.World;
using Titan.Graphics;
using Titan.UI.Common;
using Titan.UI.Components;

namespace Titan.UI
{
    public abstract class UIComponent
    {
        private Entity _entity;
        public ushort Identifier { get; set; }
        public ref readonly Entity Entity => ref _entity;
        public Size Size { get; set; }
        public Vector2 Offset { get; set; }
        public int ZIndex { get; set; }
        public AnchorPoint AnchorPoint { get; set; }
        public Vector2 Pivot { get; set; } = new (0.5f, 0.5f); // default to center
        internal virtual void OnCreate(UIManager manager, World world, in Entity entity)
        {
            //Debug.Assert(Pivot.X is >= 0 and <= 1.0f && Pivot.Y is >= 0 and <= 1.0f, "Pivot must be in the range of <0.0f 0.0f> to <1.0f 1.0f>");
            _entity = entity;
            //world.AddComponent(entity, new RectTransform
            //{
            //    Size = Size,
            //    Offset = Offset,
            //    ZIndex = ZIndex,
            //    AnchorPoint = AnchorPoint,
            //    Pivot = Vector2.Clamp(Pivot, Vector2.Zero, Vector2.One)
            //});
        }
    }
}
