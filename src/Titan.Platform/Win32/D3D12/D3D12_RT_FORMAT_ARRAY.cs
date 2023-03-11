using System.Diagnostics;
using Titan.Platform.Win32.DXGI;

namespace Titan.Platform.Win32.D3D12;

public unsafe struct D3D12_RT_FORMAT_ARRAY
{
    public fixed int RTFormats[8];
    public uint NumRenderTargets;
    public D3D12_RT_FORMAT_ARRAY(params DXGI_FORMAT[] formats) : this(formats.AsSpan()) { }
    public D3D12_RT_FORMAT_ARRAY(ReadOnlySpan<DXGI_FORMAT> formats)
    {
        Debug.Assert(formats.Length <= 8);
        fixed (int* pFormat = RTFormats)
        {
            formats.CopyTo(new Span<DXGI_FORMAT>((DXGI_FORMAT*)pFormat, 8));
        }
        NumRenderTargets = (uint)formats.Length;
    }
};
