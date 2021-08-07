using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Messaging;
using Titan.ECS.Systems;
using Titan.Graphics.Windows.Events;
using Titan.Input;
using Titan.UI.Components;

namespace Titan.UI.Systems
{
    internal class InteractableSystem : EntitySystem
    {
        private EntityFilter _filter;
        private MutableStorage<InteractableComponent> _interactable;
        private ReadOnlyStorage<RectTransform> _transform;
        
        private uint _windowHeight;

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
                if (@event.Type == WindowResizedEvent.Id)
                {
                    _windowHeight = @event.As<WindowResizedEvent>().Height;
                }
                else if (@event.Type == WindowCreatedEvent.Id)
                {
                    _windowHeight = @event.As<WindowCreatedEvent>().Height;
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
            mousePosition.Y = _windowHeight - mousePosition.Y;
            var buttonDown = InputManager.LeftMouseButtonDown;
            

            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref readonly var transform = ref _transform.Get(entity);
                ref var interactable = ref _interactable.Get(entity);
                
                // TODO: implement IsDirty
                ref readonly var rectPosition = ref transform.Position;
                ref readonly var size = ref transform.Size;
                interactable.BottomRight = new Vector2(rectPosition.X+size.Width);
                interactable.TopLeft= rectPosition;

                if (IsWithin(interactable, mousePosition))
                {
                    interactable.MouseState = MouseState.Hover;
                }
                else
                {
                    interactable.MouseState = MouseState.None;
                }
            }
        }
    }
}