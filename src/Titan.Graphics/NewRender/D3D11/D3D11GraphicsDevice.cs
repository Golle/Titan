using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Windows;
using Titan.Windows.D3D;
using Titan.Windows.D3D11;
using Titan.Windows.DXGI;
using static Titan.Windows.Common;
using static Titan.Windows.D3D.D3D_DRIVER_TYPE;
using static Titan.Windows.DXGI.DXGI_ADAPTER_FLAG;
using static Titan.Windows.DXGI.DXGI_GPU_PREFERENCE;
using static Titan.Windows.DXGI.DXGICommon;

namespace Titan.Graphics.NewRender;

internal struct D3D11Texture
{
    public ComPtr<ID3D11Texture2D> Resource;
}

internal unsafe struct D3D11GraphicsDevice //: IRenderDevice<D3D11GraphicsDevice>
{
    private MemoryPool _pool;
    private ComPtr<ID3D11Device> _device;
    private ComPtr<ID3D11DeviceContext> _context;

    public bool CreateTexture(in TextureDescriptor descriptor, out Texture outTexture)
    {
        Unsafe.SkipInit(out outTexture);

        ComPtr<ID3D11Texture2D> texture2D = default;
        var hresult = _device.Get()->CreateTexture2D(null, null, texture2D.GetAddressOf());
        if (FAILED(hresult))
        {
            Logger.Error<D3D11GraphicsDevice>($"Failed to create Texture2D with HRESULT {hresult}");
            return false;
        }

        // Allocate memory for the internal state (these can never be released... DOH)
        var texture = _pool.GetPointer<D3D11Texture>();
        outTexture.InternalState = texture;
        outTexture.ReleaseFunc = &ReleaseTexture;

        return true;
    }

    private static void ReleaseTexture(void* internalState)
    {
        var texture = (D3D11Texture*)internalState;
        if (texture != null)
        {
            texture->Resource.Release();
        }
    }



    public static bool Create(in MemoryPool pool, uint width, uint height, bool debug, out D3D11GraphicsDevice device)
    {
        device = new D3D11GraphicsDevice
        {
            _pool = pool
        };

        ComPtr<IDXGIFactory7> factory = default;
        HRESULT hr;
        if (FAILED(hr = CreateDXGIFactory1(typeof(IDXGIFactory7).GUID, (void**)factory.GetAddressOf())))
        {
            //Common.GetLastError()
            Logger.Error<D3D11GraphicsDevice>($"Failed to create {nameof(IDXGIFactory7)} with HRESULT {hr}");
            return false;
        }
        Logger.Trace<D3D11GraphicsDevice>($"{nameof(IDXGIFactory7)} created.");

        // The feature levels we'll look for
        var featureLevels = stackalloc D3D_FEATURE_LEVEL[4];
        featureLevels[0] = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_12_1;
        featureLevels[1] = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_12_0;
        featureLevels[2] = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1;
        featureLevels[3] = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0;
        D3D_FEATURE_LEVEL featureLevel = 0;

        var adapterIndex = 0u;
        using ComPtr<IDXGIAdapter1> adapter = default;
        DXGI_ADAPTER_DESC1 adapterDesc = default;
        while (factory.Get()->EnumAdapterByGpuPreference(adapterIndex++, DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE, typeof(IDXGIAdapter1).GUID, (void**)adapter.ReleaseAndGetAddressOf()) != DXGI_ERROR.DXGI_ERROR_NOT_FOUND)
        {
            adapterDesc = default;
            hr = adapter.Get()->GetDesc1(&adapterDesc);
            if (SUCCEEDED(hr))
            {
                if ((adapterDesc.Flags & DXGI_ADAPTER_FLAG_SOFTWARE) != 0)
                {
                    Logger.Info<D3D11GraphicsDevice>($"Found software adapter {adapterDesc.DescriptionString()}");
                    continue;
                }
                Logger.Info<D3D11GraphicsDevice>($"Found hardware adapter {adapterDesc.DescriptionString()}");

                var flags = debug ? D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_DEBUG : 0;
                hr = D3D11Common.D3D11CreateDevice((IDXGIAdapter*)adapter.Get(), D3D_DRIVER_TYPE_UNKNOWN, 0, flags, featureLevels, 4, D3D11Common.D3D11_SDK_VERSION, device._device.GetAddressOf(), &featureLevel, device._context.GetAddressOf());
                if (SUCCEEDED(hr))
                {
                    break;
                }
                Logger.Error($"Failed to create the D3D11Device with HRESULT {hr}");
            }
        }

        if (adapter.Get() == null)
        {
            Logger.Error<D3D11GraphicsDevice>($"Failed to find a hardware graphics adapter.");
            return false;
        }

        if (device._device.Get() == null)
        {
            Logger.Error<D3D11GraphicsDevice>($"No {nameof(ID3D11Device)} was created.");
            return false;
        }

        if (device._context.Get() == null)
        {
            Logger.Error<D3D11GraphicsDevice>($"No {nameof(ID3D11DeviceContext)} was created.");
            return false;
        }
        
        Logger.Info<D3D11GraphicsDevice>($"D3D11 Device created for Adapter {adapterDesc.DescriptionString()} and D3D Feature Level: {featureLevel}");
        return true;
    }
}
