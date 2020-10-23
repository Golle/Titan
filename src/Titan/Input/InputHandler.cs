using System;
using System.Runtime.CompilerServices;
using Titan.Core.Messaging;
using Titan.Windows.Events;

namespace Titan.Input
{
    internal class InputHandler : IInputHandler
    {
        private readonly IEventQueue _eventQueue;
        private Point _mousePosition;
        private Point _mouseLastPosition;
        private Point _mouseDeltaPosition;
        private readonly bool[] _mouseState = new bool[(int) MouseButton.Other];
        private readonly bool[] _keyState = new bool[(int) KeyCode.NumberOfKeys];
        private readonly bool[] _previousKeyState = new bool[(int) KeyCode.NumberOfKeys];

        private readonly char[] _characters = new char[1000];
        private int _characterCount;

        public bool LeftMouseButtonDown => _mouseState[(int) MouseButton.Left];
        public bool RightMouseButtonDown => _mouseState[(int) MouseButton.Right];
        public ref readonly Point MousePosition => ref _mousePosition;
        public ref readonly Point MouseLastPosition => ref _mouseLastPosition;
        public ref readonly Point MouseDeltaPosition => ref _mouseDeltaPosition;

        public InputHandler(IEventQueue eventQueue)
        {
            _eventQueue = eventQueue;
        }
        public bool IsKeyUp(KeyCode key) => !_keyState[(int) key];
        public bool IsKeyDown(KeyCode key) => _keyState[(int)key];
        public bool IsKeyPressed(KeyCode key) => IsKeyUp(key) && _previousKeyState[(int)key];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> GetCharacters() => _characterCount > 0 ? new ReadOnlySpan<char>(_characters, 0, _characterCount) : default;

        void IInputHandler.Update()
        {
            // Update mouse state
            _mouseLastPosition = _mousePosition;
            foreach (ref readonly var @event in _eventQueue.GetEvents<MouseMovedEvent>())
            {
                _mousePosition = new Point(@event.X, @event.Y);
            }
            _mouseDeltaPosition = _mouseLastPosition - _mousePosition;

            foreach (ref readonly var @event in _eventQueue.GetEvents<MouseButtonEvent>())
            {
                _mouseState[(int) @event.Button] = @event.Down;
            }

            // Update the key states
            Array.Copy(_keyState, _previousKeyState, _keyState.Length);
            Array.Fill(_keyState, false);
            foreach (ref readonly var @event in _eventQueue.GetEvents<KeyEvent>())
            {
                _keyState[@event.Code] = @event.Down;
            }

            // Update the character queue
            _characterCount = 0;
            foreach (ref readonly var @event in _eventQueue.GetEvents<CharacterTypedEvent>())
            {
                _characters[_characterCount++] = @event.Character;
            }

            // Reset all key states if the windows loses focus
            if (!_eventQueue.GetEvents<WindowLostFocusEvent>().IsEmpty)
            {
                Array.Fill(_keyState, false);
                Array.Fill(_previousKeyState, false);
                _characterCount = 0;
            }
        }
    }
}
