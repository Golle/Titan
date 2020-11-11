using System;
using Titan.Windows;

namespace Titan.Graphics.Camera
{
    public interface ICameraManager
    {
        Camera GetCamera();

    }

    internal class CameraManager : ICameraManager
    {
        private Camera _camera;

        public CameraManager(IWindow window)
        {
            _camera = new Camera(window);
        }
        public Camera GetCamera()
        {
            return _camera;
        }
    }
}
