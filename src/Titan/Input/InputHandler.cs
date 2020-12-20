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
            _characterCount = 0;
            _mouseLastPosition = _mousePosition;
            Array.Fill(_previousKeyState, false);
            var resetKeyStates = false;
            foreach (ref readonly var @event in _eventQueue.GetEvents())
            {
                if (@event.Type == MouseMovedEvent.Id)
                {
                    ref readonly var movedEvent = ref @event.As<MouseMovedEvent>();
                    _mousePosition = new Point(movedEvent.X, movedEvent.Y);
                }
                else if(@event.Type == MouseButtonEvent.Id)
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
                    // REset the key states when the window loses focus (TODO: this should be configurable)
                    resetKeyStates = true;
                }   
            }

            _mouseDeltaPosition = _mouseLastPosition - _mousePosition;
            if (resetKeyStates)
            {
                Array.Fill(_keyState, false);
                Array.Fill(_previousKeyState, false);
                _characterCount = 0;
            }
        }
    }
}
