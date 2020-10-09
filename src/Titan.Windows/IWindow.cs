using System;
using Titan.Windows.Win32;

namespace Titan.Windows
{
    public interface IWindow : IDisposable
    {
        HWND Handle { get; }
        int Height { get; }
        int Width { get; }


        void SetTitle(string title);
        void Hide();
        void Show();

        bool Update();
    }
}
