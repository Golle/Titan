using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Core.Messaging;
using Titan.Graphics.Windows;
using Titan.Graphics.Windows.Events;

namespace Titan.Input
{
    public static class InputManager
    {
        private static readonly bool[] _keyState = new bool[(int)KeyCode.NumberOfKeys];
        private static readonly bool[] _previousKeyState = new bool[(int)KeyCode.NumberOfKeys];
        private static readonly bool[] _mouseState = new bool[(int)MouseButton.Other];
        private static Vector3 _position;
        private static Vector3 _lastPosition;
        private static Vector3 _deltaPosition;
        private static readonly char[] _characters = new char[1000];
        private static int _characterCount;
        public static bool IsKeyUp(KeyCode key) => !_keyState[(int)key];
        public static bool IsKeyDown(KeyCode key) => _keyState[(int)key];
        public static bool IsKeyPressed(KeyCode key) => IsKeyUp(key) && _previousKeyState[(int)key];

        public static bool LeftMouseButtonDown => _mouseState[(int)MouseButton.Left];
        public static bool RightMouseButtonDown => _mouseState[(int)MouseButton.Right];
        public static ref readonly Vector3 MousePosition => ref _position;
        public static ref readonly Vector3 MouseLastPosition => ref _lastPosition;
        public static ref readonly Vector3 MouseDeltaPosition => ref _deltaPosition;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<char> GetCharacters() => _characterCount > 0 ? new (_characters, 0, _characterCount) : default;

        public static void Update()
        {
            Array.Clear(_previousKeyState, 0, _previousKeyState.Length);
            _characterCount = 0;
            _lastPosition = _position;

            var resetKeys = false;
            foreach (ref readonly var @event in EventManager.GetEvents())
            {
                // TODO: look at using a mask to detect events instead of a compare like this. This will be very slow for each frame (depending on the amount of events)
                if (@event.Type == MouseMovedEvent.Id)
                {
                    ref readonly var movedEvent = ref @event.As<MouseMovedEvent>();
                    var windowHeight = (int)Window.Height;
                    _position = new Vector3(movedEvent.X,  windowHeight - movedEvent.Y, 0);
                }
                else if (@event.Type == MouseButtonEvent.Id)
                {
                    ref readonly var buttonEvent = ref @event.As<MouseButtonEvent>();
                    _mouseState[(int)buttonEvent.Button] = buttonEvent.Down;
                }
                else if (@event.Type == KeyEvent.Id)
                {
                    ref readonly var keyEvent = ref @event.As<KeyEvent>();
                    ref var state = ref _keyState[keyEvent.Code];
                    _previousKeyState[keyEvent.Code] = state;
                    state = keyEvent.Down;
                }
                else if (@event.Type == CharacterTypedEvent.Id)
                {
                    _characters[_characterCount++] = @event.As<CharacterTypedEvent>().Character;
                }
                else if (@event.Type == WindowLostFocusEvent.Id)
                {
                    resetKeys = true;
                }
            }
            _deltaPosition = _lastPosition - _position;
            if (resetKeys)
            {
                Array.Clear(_previousKeyState, 0, _previousKeyState.Length);
                Array.Clear(_keyState, 0, _keyState.Length);
                _characterCount = 0;
            }
            
        }
    }
}
