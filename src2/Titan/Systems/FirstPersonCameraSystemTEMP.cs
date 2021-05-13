using System;
using System.Numerics;
using Titan.Components;
using Titan.Core.Logging;
using Titan.Core.Math;
using Titan.ECS.Entities;
using Titan.ECS.Systems;
using Titan.Graphics;
using Titan.Input;
using Titan.Rendering;

namespace Titan.Systems
{
    internal class CameraSystem : EntitySystem
    {
        private readonly GraphicsSystem _graphicsSystem;
        private readonly SimpleRenderQueue _queue;

        // TODO: read this from configuration?
        private static readonly Vector3 Forward = Vector3.UnitZ;
        private static readonly Vector3 Up = Vector3.UnitY;

        private MutableStorage<CameraComponent> _camera;
        private ReadOnlyStorage<Transform3D> _transform;
        private EntityFilter _filter;

        public CameraSystem(GraphicsSystem graphicsSystem)
        {
            _graphicsSystem = graphicsSystem; // TODO: replace with publish event
        }
        protected override void Init()
        {
            _camera = GetMutable<CameraComponent>();
            _transform = GetReadOnly<Transform3D>();
            _filter = CreateFilter(new EntityFilterConfiguration().With<CameraComponent>().With<Transform3D>());
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            var active = Entity.Null;
            
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref readonly var transform = ref _transform.Get(entity);
                ref var camera = ref _camera.Get(entity);

                // Go through components until the first camera is found
                //if (camera.Active)
                {
                    var forward = Vector3.Transform(Forward, transform.Rotation);
                    var up = Vector3.Transform(Up, transform.Rotation);
                    camera.View = Matrix4x4.CreateLookAt(transform.Position, transform.Position + forward, up);
                    camera.ViewProjection = camera.View * camera.Projection;
                    active = entity;
                    break;// TODO : add some property that will determine the "main" camera
                }
            }

            if (active != Entity.Null)
            {
                ref readonly var camera = ref _camera.Get(active);
                _graphicsSystem.SetCamera(camera.View, camera.ViewProjection);
            }
        }
    }

    public struct CameraComponent
    {
        public bool Active;

        internal int Width;
        internal int Height;
        internal float Near;
        internal float Far;


        internal Matrix4x4 View;
        internal Matrix4x4 Projection;
        internal Matrix4x4 ViewProjection;


        public static CameraComponent CreatePerspective(int width, int height, float near = 0.5f, float far = 1000f)
        {
            return new()
            {
                Active = true,
                Width = width,
                Height = height,
                Near = near,
                Far = far,
                Projection = MatrixExtensions.CreatePerspectiveLH(1f, height / (float) width, near, far)
            };
        }
    }

    internal class FirstPersonCameraSystem : EntitySystem
    {
        private MutableStorage<Transform3D> _transform;

        private bool _firstPerson;
        private Vector2 _rotation = Vector2.Zero;
        private EntityFilter _filter;
        private MutableStorage<CameraComponent> _camera;


        //public override void OnPreUpdate()
        //{
        //    if (_input.IsKeyPressed(KeyCode.Space))
        //    {
        //        _window.ToggleMouse();

        //        _firstPerson = !_firstPerson;
        //    }
        //}

        protected override void Init()
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<CameraComponent>().With<Transform3D>());
            _camera = GetMutable<CameraComponent>();
            _transform = GetMutable<Transform3D>();
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            //if (!_firstPerson)
            //{
            //    return;
            //}
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
                    Logger.Warning("moving mouse");
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
