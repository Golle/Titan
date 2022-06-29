using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Titan.Core;
using Titan.Core.Modules;
using Titan.Windows.D3D11;

namespace Titan.Graphics;



public struct AdapterInfo
{

}

public unsafe struct VulkanDevice 
{
    
}


[StructLayout(LayoutKind.Explicit)]
public unsafe struct RenderDevice
{
    [FieldOffset(0)]
    public RenderAPI API;
    [FieldOffset(sizeof(RenderAPI))]
    public ID3D11Device* D3DDevice;

    // NOTE(Jens): not implemented, just for show :)
    [FieldOffset(sizeof(RenderAPI))]
    public uint* VulkanDevice;
    [FieldOffset(sizeof(RenderAPI))]

    public int* a;


    public void Add(in int b) => a[10] = b;

}

public enum RenderAPI
{
    D3D11,
    D3D12,
    Vulkan
}
public struct RenderModule : IModule
{
    public static unsafe void Build(IApp app)
    {
        var devicePtr = (ID3D11Device*)NativeMemory.Alloc(1000);
        app.AddResource(new RenderDevice
        {
            API = RenderAPI.D3D11,
            D3DDevice = devicePtr
        });


        app.AddDisposable(new DisposableAction(() => NativeMemory.Free(devicePtr)));

    }
}


public static unsafe class D3D11Device
{
    public static ID3D11Device* CreateDevice()
    {
        return null;
    }
}
