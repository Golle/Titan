using System;
using Titan.Core.Messaging;
using Titan.Graphics.Windows.Events;

namespace Titan.Input
{
    public static class InputManager
    {
        private static readonly bool[] KeyState = new bool[(int)KeyCode.NumberOfKeys];
        private static readonly bool[] PreviousKeyState = new bool[(int)KeyCode.NumberOfKeys];

        public static bool IsKeyUp(KeyCode key) => !KeyState[(int)key];
        public static bool IsKeyDown(KeyCode key) => KeyState[(int)key];
        public static bool IsKeyPressed(KeyCode key) => IsKeyUp(key) && PreviousKeyState[(int)key];

        internal static void Update()
        {
            Array.Clear(PreviousKeyState, 0, PreviousKeyState.Length);

            var resetKeys = false;
            foreach (ref readonly var @event in EventManager.GetEvents())
            {
                if(@event.Type == KeyEvent.Id)
                {
                    ref readonly var keyEvent = ref @event.As<KeyEvent>();
                    ref var state = ref KeyState[keyEvent.Code];
                    PreviousKeyState[keyEvent.Code] = state;
                    state = keyEvent.Down;
                }
                else if (@event.Type == WindowLostFocusEvent.Id)
                {
                    resetKeys = true;
                }
            }

            if (resetKeys)
            {
                Array.Clear(PreviousKeyState, 0, PreviousKeyState.Length);
                Array.Clear(KeyState, 0, KeyState.Length);
            }
            
        }
    }
}
