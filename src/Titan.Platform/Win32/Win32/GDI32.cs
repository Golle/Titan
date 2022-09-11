using System.Runtime.InteropServices;
using Titan.Platform.Win32.DXGI;

namespace Titan.Platform.Win32.Win32
{
    public static unsafe class GDI32
    {
        [DllImport("gdi32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TextOutW(HDC hdc, int x, int y, char* lpwString, int c);

        [DllImport("gdi32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TextOutA(HDC hdc, int x, int y, byte* lpString, int c);

        [DllImport("gdi32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int SetTextColor(
            [In] HDC hdc,
            [In] COLORREF color
        );

        [DllImport("gdi32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int SetBkMode(
            [In] HDC hdc,
            [In] BackgroundMode mode
        );

        [DllImport("gdi32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern COLORREF SetBkColor(
            [In] HDC hdc,
            [In] COLORREF color
        );

        [DllImport("gdi32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BeginPath(
            [In] HDC hdc
        );

        [DllImport("gdi32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EndPath(
            [In] HDC hdc
        );


        [DllImport("gdi32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern HFONT CreateFontW(
            [In] int cHeight,
            [In] int cWidth,
            [In] int cEscapement,
            [In] int cOrientation,
            [In] int cWeight,
            [In] int bItalic,
            [In] int bUnderline,
            [In] int bStrikeOut,
            [In] int iCharSet,
            [In] int iOutPrecision,
            [In] int iClipPrecision,
            [In] int iQuality,
            [In] int iPitchAndFamily,
            [In] char* pszFaceName
        );

        [DllImport("gdi32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern HGDIOBJ SelectObject(
            [In] HDC hdc,
            [In] HGDIOBJ h
        );

        [DllImport("gdi32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(
            [In] HGDIOBJ ho
        );

        [DllImport("user32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int FillRect(
            [In] HDC hDC,
            [In] RECT* lprc,
            [In] HBRUSH hbr
        );

        [DllImport("gdi32", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern HBRUSH CreateSolidBrush(
            [In] COLORREF color
        );
    }
}

