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
        private bool _enabled;

        public DebugRenderer(ComPtr<IDXGISurface1> surface)
        {
            _surface = new ComPtr<IDXGISurface1>(surface);

            const string fontName = "Segoe UI Light";
            fixed (char* pFont = fontName)
            {
                _font = GDI32.CreateFontW(20, 0, 0, 0, 20, 0, 0, 0, 0, 0, 0, 0, 0, pFont);
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
            GDI32.SetBkColor(hdc, new COLORREF(40, 40, 0));
            GDI32.SetBkMode(hdc, BackgroundMode.Transparent);

            //var oldBrush = GDI32.SelectObject(hdc, _brush);
            var rect = new RECT
            {
                Top = 0,
                Left = 0,
                Right = 400,
                Bottom = 250
            };
            GDI32.FillRect(hdc, &rect, _brush);


            var obj = GDI32.SelectObject(hdc, _font);

            //const string str = "Sample Data collection: {0}";


            const string template = "{0}: {1:N6}ms";
            var i = 1;
            foreach (var (name, value) in EngineStats.GetStats())
            {
                var str = string.Format(template, name, value);
                fixed (char* pStr = str)
                {
                    GDI32.TextOutW(hdc, 10, i * 25, pStr, str.Length);
                    i++;
                }
            }
            
            //Span<byte> strBytes = stackalloc byte[256];
            //var r = new Random(123123);
            //for (var i = 0; i < 10; ++i)
            //{
            //    var formattedString1 = string.Format(str, r.Next(1000, 1000000));
            //    var length = Encoding.UTF8.GetBytes(formattedString1, strBytes);
            //    fixed (byte* pStr = strBytes)
            //    {
            //        GDI32.TextOutA(hdc, 10, 10 + i*20, pStr, length);
            //    }
            //}
            
            GDI32.SelectObject(hdc, obj);
            _surface.Get()->ReleaseDC(null);
        }

        public override void Dispose()
        {
            _surface.Get()->Release();
        }
    }
}
