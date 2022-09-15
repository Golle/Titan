using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.DXGI;

public struct DXGI_SWAP_CHAIN_DESC1
{
    public uint Width;
    public uint Height;
    public DXGI_FORMAT Format;
    [MarshalAs(UnmanagedType.Bool)]
    public bool Stereo;
    public DXGI_SAMPLE_DESC SampleDesc;
    public DXGI_USAGE BufferUsage;
    public uint BufferCount;
    public DXGI_SCALING Scaling;
    public DXGI_SWAP_EFFECT SwapEffect;
    public DXGI_ALPHA_MODE AlphaMode;
    public DXGI_SWAP_CHAIN_FLAG Flags;
}
