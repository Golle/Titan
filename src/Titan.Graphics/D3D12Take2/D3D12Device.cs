using System;
using System.Runtime.InteropServices.ComTypes;
using Titan.Core.Logging;
using Titan.Core.Memory;
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


public unsafe struct D3D12Buffer
{
    private ComPtr<ID3D12Resource> _resource;

    public void Release() => _resource.Reset();
}

internal unsafe struct D3D12Resource
{
    private ComPtr<ID3D12Resource> _resource;



    public void Release()
    {
        _resource.Reset();
    }
}

internal unsafe struct D3D12Heap
{
    private ComPtr<ID3D12Heap> _heap;

    public static implicit operator ID3D12Heap*(in D3D12Heap heap) => heap._heap.Get();
}

internal unsafe struct D3D12Device
{
    private ID3D12Device4* _device;


    public bool CreateBuffer(ID3D12Heap* heap)
    {
        return true;


    }
    public bool CreateBuffer(in D3D12Heap heap)
    {

        _device->CreatePlacedResource(heap, 0, null, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COPY_SOURCE, null, Guid.Empty, null);



        //new D3D12_RESOURCE_DESC
        //{
        //    Flags = 
        //}
        //    _device.Get()->CreateCommittedResource()
        //_device.Get()->CreateReservedResource()

        return true;
    }


    public bool Initialize(IDXGIAdapter3* adapter, in D3D12DeviceCreationArgs args)
    {
        var hr = D3D12CreateDevice((IUnknown*)adapter, args.MinimumFeatureLevel, typeof(ID3D12Device4).GUID, (void**)MemoryUtils.AddressOf(_device));
        if (FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed to create {nameof(ID3D12Device4)} with HRESULT {hr}");
            return false;
        }

        return true;
    }

    public void Shutdown()
    {
        _device->Release();
        _device = null;
    }
    public static implicit operator ID3D12Device4*(in D3D12Device device) => device._device;
}
