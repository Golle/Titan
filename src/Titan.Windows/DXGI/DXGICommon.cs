using System;
using System.Runtime.InteropServices;

namespace Titan.Windows.DXGI;

public static unsafe class DXGICommon
{
    [DllImport("dxgi", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT CreateDXGIFactory1(in Guid riid, void** ppFactory);
}
