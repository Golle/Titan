using System;
using Titan.Core;
using Titan.ECS.App;
using Titan.Graphics.Modules;
using Titan.Graphics.NewRender.D3D11;

namespace Titan.Graphics.NewRender;

//NOTE(Jens): This is just some POC code, remove later.
public unsafe struct Texture
{
    internal void* InternalState;
    internal delegate* <void*, void> ReleaseFunc;

    internal void Release() => ReleaseFunc(InternalState);
}

public struct TextureDescriptor { }


internal interface IRenderDevice<T> where T : unmanaged
{
    //static abstract bool Create(in MemoryPool pool, uint width, uint height, bool debug, out T device);
    
    Handle<SwapChain> CreateSwapChain(in SwapChainDescriptor descriptor);
    Handle<Texture> CreateTexture(in TextureDescriptor desc);
    
    
    void ReleaseSwapChain(in Handle<SwapChain> handle);
    void ReleaseTexture(in Handle<Texture> handle);
}

internal unsafe struct SwapChain
{
    public bool Vsync;
    public bool Fullscreen;

    internal void* InternalState;

}

internal struct SwapChainDescriptor
{
    // Add stuff
}
public unsafe struct GraphicsDevice
{
    public RenderAPI RenderApi;
    private D3D11GraphicsDevice _device;

    //NOTE(Jens): We should probably use a void* for the device and function pointers here to support other render APIs. But we're just using D3D11 now so lets hardcode that for simplicity.
    //private delegate*<void*, ref SwapChain, void> _releaseSwapChain;
    public bool CreateTexture(in TextureDescriptor textureDescriptor, out Texture texture)
    {
        return _device.CreateTexture(textureDescriptor, out texture);
    }


    //internal SwapChain CreateSwapChain(in SwapChainDescriptor descriptor, nuint windowHandle)
    //    => _device

    //internal void ReleaseSwapChain(ref SwapChain swapChain)
    //    => _releaseSwapChain(_device, ref swapChain);

    //internal static GraphicsDevice Create<T>(in MemoryPool pool, uint width, uint height, bool debug) where T : unmanaged, IRenderDevice<T>
    //{
    //    var device = T.Create(pool, width, height, debug);
    //    if (device == null)
    //    {
    //        //NOTE(Jens): We don't have any proper error handling with message boxes yet. We just throw an exception for now.
    //        throw new Exception($"Failed to create the {nameof(GraphicsDevice)}");
    //    }


    //    return new GraphicsDevice
    //    {
    //        _createSwapChain = null,
    //        _releaseSwapChain = &FunctionWrapper<T>.ReleaseSwapChain
    //    }




    //}

    internal static GraphicsDevice Create(RenderAPI renderApi, uint width, uint height)
    {
        GraphicsDevice device = default;
        //var result = renderApi switch
        //{
        //    //RenderAPI.D3D11 => D3D11GraphicsDevice.Create(width, height, out device._device),
        //    _ => throw new NotSupportedException($"Render API {renderApi} is not supported")
        //};
        //if (!result)
        //{
        //    throw new InvalidOperationException("Failed to create the device.");
        //}
        return device;
    }
}


internal unsafe struct CommandList { } // transient memory!

internal enum RenderPassAttachmentLoadOperation : byte
{
    Unknown,
    Load,
    Clear
}


internal enum RenderPassAttachmentStoreOperation : byte
{
    Unknown,
    Store
}

internal enum RenderPassAttachmentType : byte
{
    RenderTarget,
    DepthBuffer,
}

internal unsafe struct RenderPassAttachment
{
    public RenderPassAttachmentLoadOperation Load;
    public RenderPassAttachmentStoreOperation Store;
    public RenderPassAttachmentType Type;

    //public Handle<TextureNew> Texture;
}
internal unsafe struct RenderPass
{
    public RenderPassAttachment* Attachments;
    public int AttachmentsCount;

    public readonly ReadOnlySpan<RenderPassAttachment> GetAttachments()
        => new(Attachments, AttachmentsCount);
}

internal unsafe struct Renderer
{
    private void* _context;
    //private delegate*<void*, in SwapChain, void> _present;
    public void BeginRenderPass(in RenderPass renderPass) { }
    public void EndRenderPass(in RenderPass renderPass) { }
    //public void Present(in SwapChain swapChain) => _present(_context, swapChain);

    //public static Renderer Create<T>(in MemoryPool pool) where T : unmanaged, IRenderer<T>
    //{
    //    var context = T.Create(pool);
    //    return new()
    //    {
    //        _context = context,
    //        _present = &RendererFunction<T>.Present
    //    };
    //}

    /// <summary>
    /// Workaround to change the signature to void* for the context
    /// This looks nicer but needs investigation if it creates any additional code gen
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //internal struct RendererFunction<T> where T : unmanaged, IRenderer<T>
    //{
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static void Present(void* context, in SwapChain swapChain) => T.Present((T*)context, swapChain);
    //}

}

internal unsafe interface IRenderer<T> where T : unmanaged
{
    //static abstract void Present(T* context, in SwapChain swapChain);
    //static abstract T* Create(in MemoryPool pool);
}

public enum RenderAPI
{
    D3D11,
    D3D12,
    Vulkan
}
public struct RenderModule : IModule
{
    public static unsafe void Build(AppBuilder builder)
    {
        ref readonly var window = ref builder.GetResource<Window>();


        var s = sizeof(RenderPassAttachment);

        var device = GraphicsDevice.Create(RenderAPI.D3D11, window.Height, window.Width);

        //var swapChain = device.CreateSwapChain(window.Handle);

        //var device = GraphicsDevice.Create(RenderAPI.D3D11, window.Height, window.Width);


        //var texture = DeviceApi.CreateTexture(device, textureDesc)

        //device.CreateTexture();


        //var devicePtr = (ID3D11Device*)NativeMemory.Alloc(1000);
        //builder.AddResource(new RenderDevice
        //{
        //    API = RenderAPI.D3D11,
        //    D3DDevice = devicePtr
        //});
        //builder.AddSystemToStage<RenderDeviceTeardown>(Stage.Shutdown);
    }

    //private struct RenderDeviceTeardown : IStructSystem<RenderDeviceTeardown>
    //{
    //    private MutableResource<RenderDevice> _renderDevice;
    //    public static void Init(ref RenderDeviceTeardown system, in SystemsInitializer init) => system._renderDevice = init.GetMutableResource<RenderDevice>();
    //    public static unsafe void Update(ref RenderDeviceTeardown system) => NativeMemory.Free(system._renderDevice.Get().D3DDevice);
    //    public static bool ShouldRun(in RenderDeviceTeardown system) => true;
    //}
}
