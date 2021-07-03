using Titan.Core.Messaging;
using Titan.Graphics.Windows;
using Titan.Graphics.Windows.Events;

namespace Titan
{
    public class GameWindow
    {
        private readonly Window _window;
        public uint Height => _window.Height;
        public uint Width => _window.Width;
        public bool Windowed => _window.Windowed;
        internal GameWindow(Window window)
        {
            _window = window;
        }
        
        public void HideMouse()
        {
            EventManager.Push(new MouseStateEvent(false));
        }

        public void ShowMouse()
        {
            EventManager.Push(new MouseStateEvent(true));
        }

        
    }
}
