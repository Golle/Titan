using System.Linq;
using Titan.Core;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Input;
using Titan.Windows;
using Titan.Windows.D3D11;
using Titan.Windows.Win32;

namespace Titan.Rendering
{
    internal sealed unsafe class DebugRenderer : Renderer
    {
        private readonly ComPtr<IDXGISurface1> _surface;
        private HFONT _font;
        private HBRUSH _brush;
        private bool _enabled = true;

        public DebugRenderer(ComPtr<IDXGISurface1> surface)
        {
            _surface = new ComPtr<IDXGISurface1>(surface);

            //const string fontName = "Segoe UI Light";
            const string fontName = "Courier";
            fixed (char* pFont = fontName)
            {
                _font = GDI32.CreateFontW(16, 0, 0, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, pFont);
            }

            _brush = GDI32.CreateSolidBrush(new COLORREF(50, 50, 50));
        }

        public override void Render(Context context)
        {
            if (InputManager.IsKeyPressed(KeyCode.Q))
            {
                _enabled = !_enabled;
            }

            if (!_enabled)
            {
                return;
            }
            HDC hdc;
            Common.CheckAndThrow(_surface.Get()->GetDC(0, &hdc), nameof(IDXGISurface1.GetDC));

            GDI32.SetTextColor(hdc, new COLORREF(0, 200, 200));
            GDI32.SetBkColor(hdc, new COLORREF(0, 0, 0));
            GDI32.SetBkMode(hdc, BackgroundMode.Transparent);

            const int lineHeight = 25;
            var rectSize = (EngineStats.TotalLines + 2) * lineHeight; 
            //var oldBrush = GDI32.SelectObject(hdc, _brush);
            var rect = new RECT
            {
                Top = 0,
                Left = 0,
                Right = 800,
                Bottom = rectSize
            };
            GDI32.FillRect(hdc, &rect, _brush);
            var obj = GDI32.SelectObject(hdc, _font);
            
            const string template = "{0}: {1:N6}ms";
            var i = 1;
            foreach (var (name, value) in EngineStats.GetStats())
            {
                var str = string.Format(template, name, value);
                fixed (char* pStr = str)
                {
                    GDI32.TextOutW(hdc, 10, i * lineHeight, pStr, str.Length);
                    i++;
                }
            }

            const string systems = "Systems";
            fixed (char* pStr = systems)
            {
                GDI32.TextOutW(hdc, 10, i * 25, pStr, systems.Length);
                i++;
            }

            const string systemsTemplate = "{0} Pre: {1:N4}ms   Update: {2:N4}ms   Post: {3:N4}ms";
            foreach (var (key, value) in EngineStats.GetSystemStats().OrderBy(e => e.Key))
            {
                var str = string.Format(systemsTemplate, key.PadRight(25), value.PreUpdate, value.Update, value.PostUpdate);
                fixed (char* pStr = str)
                {
                    GDI32.TextOutW(hdc, 10, i * lineHeight, pStr, str.Length);
                    i++;
                }
            }

            GDI32.SelectObject(hdc, obj);
            _surface.Get()->ReleaseDC(null);
        }

        public override void Dispose()
        {
            _surface.Get()->Release();
        }
    }
}
