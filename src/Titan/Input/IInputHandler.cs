using System;

namespace Titan.Input
{
    public interface IInputHandler
    {
        bool LeftMouseButtonDown { get; }
        bool RightMouseButtonDown { get; }
        ref readonly Point MousePosition { get; }
        ref readonly Point MouseLastPosition { get; }
        ref readonly Point MouseDeltaPosition { get; }
        
        bool IsKeyUp(KeyCode key);
        bool IsKeyDown(KeyCode key);
        bool IsKeyPressed(KeyCode key);
        ReadOnlySpan<char> GetCharacters();

        internal void Update();
    }
}
