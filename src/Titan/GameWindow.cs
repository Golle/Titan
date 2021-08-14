using Titan.Core.Messaging;
using Titan.Graphics.Windows;
using Titan.Graphics.Windows.Events;

namespace Titan
{
    public class GameWindow
    {
        public uint Height => Window.Height;
        public uint Width => Window.Width;
        public bool Windowed => Window.Windowed;
       
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
