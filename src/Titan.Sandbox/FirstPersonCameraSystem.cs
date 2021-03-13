using System;
using System.Numerics;
using Titan.ECS.Components;
using Titan.ECS.Systems;
using Titan.ECS.World;
using Titan.EntitySystem.Components;
using Titan.Input;
using Titan.Windows;

namespace Titan.Sandbox
{
    internal class FirstPersonCameraSystem : SystemBase
    {
        private readonly IInputHandler _input;
        private readonly IWindow _window;
        private readonly IEntityFilter _filter;
        private readonly MutableStorage<Transform3D> _transform;

        private bool _firstPerson;
        public FirstPersonCameraSystem(IWorld world, IEntityFilterManager entityFilterManager, IInputHandler inputHandler, IWindow window) : base(world)
        {
            _input = inputHandler;
            _window = window;
            _filter = entityFilterManager.Create(new EntityFilterConfiguration().With<CameraComponent>().With<Transform3D>());

            _transform = GetMutable<Transform3D>();

        }


        private Vector2 _rotation = Vector2.Zero;


        public override void OnPreUpdate()
        {
            if (_input.IsKeyPressed(KeyCode.Space))
            {
                _window.ToggleMouse();

                _firstPerson = !_firstPerson;
            }
        }

        public override void OnUpdate()
        {
            if (!_firstPerson)
            {
                return;
            }
            const float maxRotation = (float)Math.PI / 2f - 0.01f;
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var transform = ref _transform.Get(entity);
                //transform.Rotation = Quaternion.CreateFromYawPitchRoll(0,0,0);


                var multiplier = _input.IsKeyDown(KeyCode.Shift) ? 15f : 1f;
                var speed = 0.2f * multiplier;
                var delta = _input.MouseLastPosition - _input.MousePosition;
                if (delta.Y != 0 || delta.X != 0)
                {
                    const float constant = 0.003f;
                    _rotation.X -= delta.X * constant;
                    _rotation.Y = Math.Clamp(_rotation.Y + delta.Y * constant, -maxRotation, maxRotation);
                    transform.Rotation = Quaternion.CreateFromYawPitchRoll(_rotation.X, _rotation.Y, 0f);
                    //camera.Rotate(new Vector3(rotation, 0)); // not required?
                }

                var position = transform.Position;
                if (_input.IsKeyDown(KeyCode.W))
                {
                    position += Vector3.Transform(Vector3.UnitZ * -speed, transform.Rotation);
                }
                if (_input.IsKeyDown(KeyCode.S))
                {
                    position += Vector3.Transform(Vector3.UnitZ * speed, transform.Rotation);
                }
                if (_input.IsKeyDown(KeyCode.A))
                {
                    position += Vector3.Transform(Vector3.UnitX * speed, transform.Rotation);
                }
                if (_input.IsKeyDown(KeyCode.D))
                {
                    position += Vector3.Transform(Vector3.UnitX * -speed, transform.Rotation);
                }
                if (_input.IsKeyDown(KeyCode.V))
                {
                    //camera.MoveUp(speed);
                }
                if (_input.IsKeyDown(KeyCode.C))
                {
                    //camera.MoveUp(-speed);
                }
                transform.Position = position;


                
                //    }

            }
        }
    }
}
