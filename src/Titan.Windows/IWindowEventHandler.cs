using Titan.Core.Logging;
using Titan.Windows.Win32;

namespace Titan.Windows
{
    internal interface IWindowEventHandler
    {
        void OnCreate();
        void OnClose();
        void OnLostFocus();
        void OnKeyDown(KeyCode code, bool repeat);
        void OnKeyUp(KeyCode code);
        void OnCharTyped(char character);
        void OnWindowResize(int width, int height);
    }

    internal class WindowEventHandler : IWindowEventHandler
    {
        public WindowEventHandler()
        {
            
        }

        public void OnCreate()
        {
            LOGGER.Debug("Window created");
        }

        public void OnClose()
        {
            LOGGER.Debug("Window closed");
        }

        public void OnLostFocus()
        {
            LOGGER.Debug("Window lost focus");
        }

        public void OnKeyDown(KeyCode code, bool repeat)
        {
            LOGGER.Debug($"Key {code} down. Repeat: {repeat}");
        }

        public void OnKeyUp(KeyCode code)
        {
            LOGGER.Debug($"Key {code} up.");
        }

        public void OnCharTyped(char character)
        {
            LOGGER.Debug($"Char typed {character}");
        }

        public void OnWindowResize(int width, int height)
        {
            LOGGER.Debug($"Window resized to {width} x {height}");
        }
    }
}
