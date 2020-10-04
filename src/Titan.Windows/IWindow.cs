using System;

namespace Titan.Windows
{
    public interface IWindow : IDisposable
    {
        nint NativeHandle { get; }
        int Height { get; }
        int Width { get; }


        void SetTitle(string title);
        void Hide();
        void Show();

        bool Update();
    }
}
