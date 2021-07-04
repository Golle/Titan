using System;
using System.Numerics;
using Titan.Components;
using Titan.ECS.Systems;
using Titan.Input;

namespace Titan.Sandbox
{
   

    internal class FirstPersonCameraSystem : EntitySystem
    {
        private readonly GameWindow _window;

        private MutableStorage<Transform3D> _transform;
        private bool _firstPerson;
        private Vector2 _rotation = Vector2.Zero;
        private EntityFilter _filter;
        private MutableStorage<CameraComponent> _camera;

        public FirstPersonCameraSystem(GameWindow window)
        {
            _window = window;
        }

        protected override void OnPreUpdate()
        {
            if (InputManager.IsKeyPressed(KeyCode.Space))
            {
                
                _firstPerson = !_firstPerson;
                if (_firstPerson)
                {
                    _window.HideMouse();
                }
                else
                {
                    _window.ShowMouse();
                }
            }
        }

        protected override void Init()
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<CameraComponent>().With<Transform3D>());
            _camera = GetMutable<CameraComponent>();
            _transform = GetMutable<Transform3D>();
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            if (!_firstPerson)
            {
                return;
            }
            const float maxRotation = (float) Math.PI / 2f - 0.01f;
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var transform = ref _transform.Get(entity);
                //transform.Rotation = Quaternion.CreateFromYawPitchRoll(0,0,0);

                var multiplier = InputManager.IsKeyDown(KeyCode.Shift) ? 15f : 1f;
                var speed = 0.2f * multiplier;
                var delta = InputManager.MouseDeltaPosition;
                if (delta.Y != 0 || delta.X != 0)
                {
                    const float constant = 0.003f;
                    _rotation.X -= delta.X * constant;
                    _rotation.Y = Math.Clamp(_rotation.Y + delta.Y * constant, -maxRotation, maxRotation);
                    transform.Rotation = Quaternion.CreateFromYawPitchRoll(_rotation.X, _rotation.Y, 0f);
                    //camera.Rotate(new Vector3(rotation, 0)); // not required?
                }

                var position = transform.Position;
                if (InputManager.IsKeyDown(KeyCode.W))
                {
                    position += Vector3.Transform(Vector3.UnitZ * -speed, transform.Rotation);
                }

                if (InputManager.IsKeyDown(KeyCode.S))
                {
                    position += Vector3.Transform(Vector3.UnitZ * speed, transform.Rotation);
                }

                if (InputManager.IsKeyDown(KeyCode.A))
                {
                    position += Vector3.Transform(Vector3.UnitX * speed, transform.Rotation);
                }

                if (InputManager.IsKeyDown(KeyCode.D))
                {
                    position += Vector3.Transform(Vector3.UnitX * -speed, transform.Rotation);
                }

                if (InputManager.IsKeyDown(KeyCode.V))
                {
                    //camera.MoveUp(speed);
                }

                if (InputManager.IsKeyDown(KeyCode.C))
                {
                    //camera.MoveUp(-speed);
                }

                transform.Position = position;
            }
        }
    }
}
