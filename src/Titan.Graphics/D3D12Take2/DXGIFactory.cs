using Titan.Core.Logging;
using Titan.Windows;
using Titan.Windows.DXGI;
using static Titan.Windows.Common;
using static Titan.Windows.DXGI.DXGI_CREATE_FACTORY_FLAGS;
using static Titan.Windows.DXGI.DXGICommon;

namespace Titan.Graphics.D3D12Take2;

internal unsafe struct DXGIFactory
{
    private ComPtr<IDXGIFactory7> _factory;
    public bool Initialize(bool debug)
    {
        var flags = debug ? DXGI_CREATE_FACTORY_DEBUG : 0;
        Logger.Trace<DXGIFactory>($"Create {nameof(IDXGIFactory7)} with flags: {flags}");
        var hr = CreateDXGIFactory2(flags, typeof(IDXGIFactory7).GUID, (void**)_factory.ReleaseAndGetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<DXGIFactory>($"Failed to create {nameof(IDXGIFactory7)} with HRESULT {hr}");
            return false;
        }

        return true;
    }

    public void Shutdown()
    {
        _factory.Release();
    }


    public static implicit operator IDXGIFactory7*(in DXGIFactory factory) => factory._factory.Get();
}
