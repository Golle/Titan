using Titan.Windows.Win32;
using Titan.Windows.Win32.Native;

namespace Titan.Windows
{
    internal interface IWindowEventHandler
    {
        void OnCreate();
        void OnClose();
        void OnLostFocus();
        void OnKeyDown(nuint wParam, nuint lParam);
        void OnKeyUp(nuint code);
        void OnCharTyped(nuint character);
        void OnWindowResize(int width, int height);
        void OnLeftMouseButtonDown();
        void OnLeftMouseButtonUp();
        void OnRightMouseButtonDown();
        void OnRightMouseButtonUp();
        void OnMouseMove(in POINT position);
        
    }
}
