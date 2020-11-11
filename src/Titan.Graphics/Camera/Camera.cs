using System;
using System.Numerics;
using Titan.Core.Common;
using Titan.Windows;

namespace Titan.Graphics.Camera
{
    public interface ICamera
    {
        ref readonly Vector3 Position { get; }
        ref readonly Matrix4x4 View { get; }
        ref readonly Matrix4x4 ViewProjection { get; }

        void MoveForward(float distance);
        void MoveSide(float distance);
        void MoveUp(float distance);
        void Rotate(in Vector3 rotation);
        void Update();

    }
    public class Camera : ICamera
    {
        public ref readonly Vector3 Position => ref _position;
        public ref readonly Matrix4x4 View => ref _view;
        public ref readonly Matrix4x4 ViewProjection => ref _viewProjection;
        public void MoveForward(float distance)
        {
            _position += Vector3.Transform(Vector3.UnitZ * distance, _rotation);
        }

        public void MoveSide(float distance)
        {
            _position += Vector3.Transform(Vector3.UnitX * distance, _rotation);
        }

        public void MoveUp(float distance)
        {
            _position += Vector3.Transform(Vector3.UnitY * distance, _rotation);
            
        }

        public void Rotate(in Vector3 rotation)
        {
            _rotation = Quaternion.CreateFromYawPitchRoll(rotation.X, rotation.Y, 0);
            
        }

        private Vector3 _position;
        private Matrix4x4 _view;
        private Matrix4x4 _viewProjection;
        private Matrix4x4 _projection;
        private Quaternion _rotation;

        private static readonly Vector3 Forward = Vector3.UnitZ;
        private static readonly Vector3 Up = Vector3.UnitY;

        public Camera(IWindow window)
        {
            _position = new Vector3(0, 0, 0);
            _projection = MatrixExtensions.CreatePerspectiveLH(1f, window.Height / (float)window.Width, 0.5f, 10000f);
            _rotation = Quaternion.CreateFromYawPitchRoll(0, 0, 0);
            
            //var viewProjectionMatrix = new Matrix4x4(-1, 0, 0, 0, 0, 1.77777779f, 0, 0, 0, 0, -1.00005f, -1, 0, 0, -0.5f, 0);
        }

        private float rot;
        public void Update()
        {
            var forward = Vector3.Transform(Forward, _rotation);
            var up = Vector3.Transform(Up, _rotation);
            //position += Vector3.Transform(distance, rotation);
            
            _view = Matrix4x4.CreateLookAt(_position, _position + forward, up);
            _viewProjection = _view * _projection;
        }
    }
}
