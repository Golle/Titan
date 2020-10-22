using Titan.Core.Messaging;
using Titan.Windows.Events;
using Titan.Windows.Win32;
using Titan.Windows.Win32.Native;

namespace Titan.Windows
{
    internal class WindowEventHandler : IWindowEventHandler
    {
        private readonly IEventQueue _eventQueue;

        public WindowEventHandler(IEventQueue eventQueue)
        {
            _eventQueue = eventQueue;
        }

        public void OnCreate() => _eventQueue.Push(new WindowCreatedEvent());
        public void OnClose() => _eventQueue.Push(new WindowClosedEvent());
        public void OnLostFocus() => _eventQueue.Push(new WindowLostFocusEvent());
        public void OnKeyUp(nuint code) => _eventQueue.Push(new KeyUpEvent((KeyCode)code));
        public void OnCharTyped(nuint character) => _eventQueue.Push(new CharacterTypedEvent((char)character));
        public void OnWindowResize(int width, int height) => _eventQueue.Push(new WindowResizedEvent(width, height));
        public void OnLeftMouseButtonDown() => _eventQueue.Push(new MouseButtonDownEvent(MouseButton.Left));
        public void OnLeftMouseButtonUp() => _eventQueue.Push(new MouseButtonUpEvent(MouseButton.Left));
        public void OnRightMouseButtonDown() => _eventQueue.Push(new MouseButtonDownEvent(MouseButton.Right));
        public void OnRightMouseButtonUp() => _eventQueue.Push(new MouseButtonUpEvent(MouseButton.Right));
        public void OnMouseMove(in POINT position) => _eventQueue.Push(new MouseMovedEvent(position.X, position.Y));
        public void OnKeyDown(nuint wParam, nuint lParam)
        {
            var repeat = (lParam & 0x40000000) > 0;
            var code = (KeyCode)wParam;

            _eventQueue.Push(new KeyDownEvent(code, repeat));
        }
    }
}
