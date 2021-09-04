using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Messaging;
using Titan.ECS.Systems;
using Titan.Input;
using Titan.UI.Components;

namespace Titan.UI.Systems
{
    internal class InteractableSystem : EntitySystem
    {
        private EntityFilter _filter;
        private MutableStorage<InteractableComponent> _interactable;
        private ReadOnlyStorage<RectTransform> _transform;

        public InteractableSystem() : base(-1000)
        {
        }
        protected override void Init()
        {
            
            // TOOD: this must be sorted by Z-Index
            _filter = CreateFilter(new EntityFilterConfiguration().With<RectTransform>().With<InteractableComponent>());

            _interactable = GetMutable<InteractableComponent>();
            _transform = GetReadOnly<RectTransform>();
        }


        protected override void OnPreUpdate()
        {
            foreach (ref readonly var @event in EventManager.GetEvents())
            {
                if (@event.Type == UIButtonEvent.Id)
                {
                    ref readonly var buttonEvent = ref @event.As<UIButtonEvent>();
                    ref var interactable = ref _interactable.Get(buttonEvent.Entity);
                    switch (buttonEvent.State)
                    {
                        case ButtonState.None:
                            break;
                        case ButtonState.Enter:
                            interactable.MouseState |= MouseState.Hover;
                            break;
                        case ButtonState.Leave:
                            interactable.MouseState ^= MouseState.Hover;
                            break;
                        case ButtonState.Down:
                            interactable.MouseState |= MouseState.Down;
                            break;
                        case ButtonState.Up:
                            interactable.MouseState ^= MouseState.Down;
                            break;
                    }
                    Logger.Trace<InteractableSystem>($"ButtonEvent: {buttonEvent.State} ({buttonEvent.Entity.Id}, {buttonEvent.ButtonId})");
                }
            }
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            static bool IsWithin(in InteractableComponent interactable, in Vector3 position) =>
                !(interactable.TopLeft.X > position.X ||
                  interactable.TopLeft.Y > position.Y ||
                  interactable.BottomRight.X < position.X ||
                  interactable.BottomRight.Y < position.Y);
            
            var mousePosition = InputManager.MousePosition;
            var buttonDown = InputManager.LeftMouseButtonDown;
            var buttonDownPrev = InputManager.PreviousLeftMouseButtonDown;
            

            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref readonly var transform = ref _transform.Get(entity);
                ref var interactable = ref _interactable.Get(entity);
                
                // TODO: implement IsDirty
                ref readonly var rectPosition = ref transform.AbsolutePosition;
                ref readonly var size = ref transform.Size;
                interactable.BottomRight = new Vector2(rectPosition.X + size.Width, rectPosition.Y + size.Height);
                interactable.TopLeft = rectPosition;

                var previousHover = (interactable.MouseState & MouseState.Hover) > 0;
                var previousMouseButtonDown = (interactable.MouseState & MouseState.Down) > 0;
                var isWithin = IsWithin(interactable, mousePosition);

                if (isWithin && previousHover == false)
                {
                    EventManager.Push(new UIButtonEvent(entity, interactable.Id, ButtonState.Enter));
                }

                if (previousHover && isWithin == false)
                {
                    EventManager.Push(new UIButtonEvent(entity, interactable.Id, ButtonState.Leave));
                }

                if (previousMouseButtonDown && !buttonDown)
                {
                    EventManager.Push(new UIButtonEvent(entity, interactable.Id, ButtonState.Up));
                }

                // only trigger down event if the mouse is over and is clicked in the same frame.
                if (isWithin && previousMouseButtonDown == false && buttonDown && buttonDownPrev == false)
                {
                    EventManager.Push(new UIButtonEvent(entity, interactable.Id, ButtonState.Down));
                }
            }
        }
    }
}
