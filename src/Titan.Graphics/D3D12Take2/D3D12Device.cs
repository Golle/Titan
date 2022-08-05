using Titan.Core.Logging;
using Titan.Windows;
using Titan.Windows.D3D;
using Titan.Windows.D3D12;
using Titan.Windows.DXGI;
using static Titan.Windows.Common;
using static Titan.Windows.D3D12.D3D12Common;

namespace Titan.Graphics.D3D12Take2;
internal struct D3D12DeviceCreationArgs
{
    public required nint WindowHandle;
    public required uint Width;
    public required uint Height;

    public required D3D_FEATURE_LEVEL MinimumFeatureLevel;
}

internal unsafe struct D3D12Device
{
    private ComPtr<ID3D12Device4> _device;
    public bool Initialize(IDXGIAdapter3* adapter, in D3D12DeviceCreationArgs args)
    {
        var hr = D3D12CreateDevice((IUnknown*)adapter, args.MinimumFeatureLevel, typeof(ID3D12Device4).GUID, (void**)_device.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed to create {nameof(ID3D12Device4)} with HRESULT {hr}");
            return false;
        }

        return true;
    }

    public void Shutdown()
    {
        _device.Release();
        _device = default;
    }
    public static implicit operator ID3D12Device4*(in D3D12Device device) => device._device.Get();
}
