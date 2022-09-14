using Titan.Core;
using Titan.Core.Logging;
using Titan.Graphics.D3D11.BlendStates;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Rasterizer;
using Titan.Graphics.D3D11.Samplers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D;
using Titan.Platform.Win32.D3D11;
using Titan.Platform.Win32.DXGI;
using static Titan.Platform.Win32.Common;
using static Titan.Platform.Win32.D3D11.D3D11Common;
using static Titan.Platform.Win32.DXGI.DXGI_FORMAT;
using static Titan.Platform.Win32.DXGI.DXGI_MODE_SCALING;
using static Titan.Platform.Win32.DXGI.DXGI_MODE_SCANLINE_ORDER;
using static Titan.Platform.Win32.DXGI.DXGI_SWAP_CHAIN_FLAG;
using static Titan.Platform.Win32.DXGI.DXGI_SWAP_EFFECT;
using static Titan.Platform.Win32.DXGI.DXGI_USAGE;

namespace Titan.Graphics.D3D11;

public record DeviceConfiguration(uint Width, uint Height, uint RefreshRate, bool Windowed, bool Vsync, bool Debug, bool Stats);

public static unsafe class GraphicsDevice
{
    private static ComPtr<ID3D11Device> _device;
    private static ComPtr<ID3D11DeviceContext> _context;
    private static ComPtr<IDXGISwapChain> _swapChain;
    private static Handle<Texture> _backbufferHandle;

    public static SwapChain SwapChain { get; private set; }
    public static Context ImmediateContext { get; private set; }
    public static bool IsInitialized { get; private set; }

    public static BufferManager BufferManager { get; private set; }
    public static TextureManager TextureManager { get; private set; }
    public static SamplerManager SamplerManager { get; private set; }
    public static ShaderManager ShaderManager { get; private set; }
    public static RasterizerManager RasterizerManager { get; private set; }
    public static BlendStateManager BlendStateManager { get; private set; }
    public static Handle<Texture> Backbuffer => _backbufferHandle;

    public static void Init(DeviceConfiguration config, HWND windowHandle)
    {
        if (IsInitialized)
        {
            throw new InvalidOperationException($"{nameof(GraphicsDevice)} has already been initialized.");
        }

        var deviceCreationFlags = D3D11_CREATE_DEVICE_FLAG.UNSPECIFIED;
        if (config.Debug)
        {
            deviceCreationFlags |= D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_DEBUG;
        }

        if (config.Stats)
        {
            deviceCreationFlags |= D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT;
        }

        var featureLevel = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1;
        var desc = new DXGI_SWAP_CHAIN_DESC
        {
            BufferCount = 2,
            BufferDesc = new DXGI_MODE_DESC
            {
                Width = config.Width,
                Height = config.Height,
                RefreshRate = new DXGI_RATIONAL { Denominator = config.RefreshRate },
                Scaling = DXGI_MODE_SCALING_UNSPECIFIED,
                ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED,
                Format = DXGI_FORMAT_B8G8R8A8_UNORM
            },
            SampleDesc = new DXGI_SAMPLE_DESC
            {
                //Count = 2, // TODO: look into how we should use AA/Multi Sampling in Titan
                Count = 1,
                Quality = 0
            },

            BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT,
            OutputWindow = windowHandle,
            //SwapEffect = DXGI_SWAP_EFFECT_SEQUENTIAL,
            SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD,
            Flags = DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH,
            Windowed = config.Windowed
        };
            
            
        Logger.Trace<ID3D11Device>("Creating device");
        CheckAndThrow(D3D11CreateDeviceAndSwapChain(null, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, 0, deviceCreationFlags, null, 0, D3D11_SDK_VERSION, &desc, _swapChain.GetAddressOf(), _device.GetAddressOf(), null, _context.GetAddressOf()), nameof(D3D11CreateDeviceAndSwapChain));
        Logger.Trace<ID3D11Device>("Device created");


#if DEBUG

        Logger.Trace<ID3D11Device>("Sampling quality levels");
        uint qualityLevel = 0;
        for (var sampleCount = 1u; sampleCount <= 16u; ++sampleCount)
        {
            var result = _device.Get()->CheckMultisampleQualityLevels(DXGI_FORMAT_B8G8R8A8_UNORM, sampleCount, &qualityLevel);
            if (SUCCEEDED(result))
            {
                Logger.Debug($"Sample count {sampleCount} supports quality level {qualityLevel}", typeof(GraphicsDevice));
            }
            else
            {
                Logger.Debug($"Sample count {sampleCount} failed with HRESULT {result}", typeof(GraphicsDevice));
            }
        }
#endif

        SwapChain = new SwapChain(_swapChain.Get(), config);

        ImmediateContext = new Context(_context.Get());
        BufferManager = new BufferManager(_device);
        TextureManager = new TextureManager(_device.Get(), SwapChain);
        SamplerManager = new SamplerManager(_device.Get());
        ShaderManager = new ShaderManager(_device.Get());
        RasterizerManager = new RasterizerManager(_device.Get());
        BlendStateManager = new BlendStateManager(_device.Get());

        InitBackbuffer();
        IsInitialized = true;
    }

    public static void Resize(uint width, uint height)
    {
        ReleaseBackbuffer();
        SwapChain.Resize(width, height);
        InitBackbuffer();
    }

    private static void ReleaseBackbuffer()
    {
        if (_backbufferHandle.IsValid())
        {
            TextureManager.Release(_backbufferHandle);
            _backbufferHandle = Handle<Texture>.Null;
        }
    }
    private static void InitBackbuffer()
    {
        using ComPtr<ID3D11Resource> backbufferResource = default;
        fixed (Guid* resourcePointer = &D3D11Resource)
        {
            CheckAndThrow(_swapChain.Get()->GetBuffer(0, resourcePointer, (void**)backbufferResource.GetAddressOf()), nameof(IDXGISwapChain.GetBuffer));
        }

        ID3D11RenderTargetView* backbuffer;
        CheckAndThrow(_device.Get()->CreateRenderTargetView(backbufferResource.Get(), null, &backbuffer), nameof(ID3D11Device.CreateRenderTargetView));
        
        _backbufferHandle = TextureManager.CreateManual(new Texture
        {
            Format = TextureFormats.RGBA32F,
            Width = SwapChain.Width,
            Height = SwapChain.Height,
            BindFlags = D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET,
            D3DTarget = backbuffer,
            Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE
        });
    }


    public static void Terminate()
    {
        if (IsInitialized)
        {
            Logger.Trace<ID3D11Device>("Disposing managers");
            ReleaseBackbuffer();

            BufferManager.Dispose();
            BufferManager = null;
            TextureManager.Dispose();
            TextureManager = null;
            SamplerManager.Dispose();
            SamplerManager = null;
            ShaderManager.Dispose();
            ShaderManager = null;
            RasterizerManager.Dispose();
            RasterizerManager = null;
            SwapChain.Dispose();
            SwapChain = null;

            Logger.Trace<ID3D11Device>("Disposing resources");
            _swapChain.Dispose();
            _context.Dispose();
            _device.Dispose();
        }
        IsInitialized = false;
    }
}
