using Titan.Windows.Win32;
using Titan.Windows.Win32.Native;

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
        void OnLeftMouseButtonDown();
        void OnLeftMouseButtonUp();
        void OnRightMouseButtonDown();
        void OnRightMouseButtonUp();
        void OnMouseMove(in POINT position);
    }
}
