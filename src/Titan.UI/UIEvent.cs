using Titan.Core.Messaging;
using Titan.ECS.Entities;

namespace Titan.UI
{
    public enum ButtonState : ushort
    {
        None,
        Enter,
        Leave,
        Down,
        Up
    }

    public readonly struct UIButtonEvent
    {
        public static readonly short Id = EventId<UIButtonEvent>.Value;

        public readonly Entity Entity;
        public readonly ushort ButtonId;
        public readonly ButtonState State;
        public UIButtonEvent(Entity entity, ushort buttonId, ButtonState state)
        {
            Entity = entity;
            ButtonId = buttonId;
            State = state;
        }
    }
}
