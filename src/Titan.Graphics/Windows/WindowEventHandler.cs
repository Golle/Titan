using System.Runtime.CompilerServices;
using Titan.Core.Messaging;
using Titan.Graphics.Windows.Events;
using Titan.Windows.Win32;

namespace Titan.Graphics.Windows
{
    internal class WindowEventHandler
    {
        public static void OnCreate(uint width, uint height) => EventManager.Push(new WindowCreatedEvent(width,height));
        public static void OnClose() => EventManager.Push(new WindowClosedEvent());
        public static void OnLostFocus() => EventManager.Push(new WindowLostFocusEvent());
        public static void OnKeyUp(nuint code) => EventManager.Push(new KeyEvent((int)code, false, false));
        public static void OnCharTyped(nuint character) => EventManager.Push(new CharacterTypedEvent((char)character));
        public static void OnWindowResize(uint width, uint height) => EventManager.Push(new WindowResizedEvent(width, height));
        public static void OnLeftMouseButtonDown() => EventManager.Push(new MouseButtonEvent(MouseButton.Left, true));
        public static void OnLeftMouseButtonUp() => EventManager.Push(new MouseButtonEvent(MouseButton.Left, false));
        public static void OnRightMouseButtonDown() => EventManager.Push(new MouseButtonEvent(MouseButton.Right, true));
        public static void OnRightMouseButtonUp() => EventManager.Push(new MouseButtonEvent(MouseButton.Right, false));
        public static void OnMouseMove(in POINT position) => EventManager.Push(new MouseMovedEvent(position.X, position.Y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OnKeyDown(nuint wParam, nuint lParam)
        {
            var repeat = (lParam & 0x40000000) > 0;
            var code = (int)wParam;

            EventManager.Push(new KeyEvent(code, true, repeat));
        }

        public static void OnSetFocus() => EventManager.Push(new WindowSetFocusEvent());


        //public static void OnGamepadArrival(nuint handle);
        //public static void OnGamepadRemoved(nuint handle);

    }
}
