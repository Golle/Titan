using System;
using System.Diagnostics;
using System.Threading;
using Titan.Assets.NewAssets;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.App;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;
using Titan.Graphics.D3D12Take2.Stats;
using Titan.Graphics.D3D12Take2.Systems;
using Titan.Graphics.Modules;
using Titan.Graphics.NewRender;
using Titan.Windows;
using Titan.Windows.D3D;
using Titan.Windows.D3D12;
using Titan.Windows.DXGI;

namespace Titan.Graphics.D3D12Take2;


public unsafe struct D3D12RenderModule1 : IModule
{
    private const uint BufferCount = 3;
    public static bool Build(AppBuilder builder)
    {
        ref readonly var window = ref builder.GetResource<Window>();

        const bool Debug = true;
        // add some config that we can check if debug should be enabled
        if (Debug)
        {
            EnableDebugLayer();
        }


        DXGIFactory factory = default;
        DXGIAdapter adapter = default;
        D3D12Device device = default;
        D3D12Surface surface = default;
        D3D12GraphicsQueue queue = default;
        D3D12Command command = default;

        if (!factory.Initialize(Debug))
        {
            goto Error;
        }
        if (!adapter.Initialize(factory))
        {
            goto Error;
        }

        // Create the device
        var deviceArgs = new D3D12DeviceCreationArgs
        {
            Height = window.Height,
            Width = window.Width,
            WindowHandle = window.Handle,
            MinimumFeatureLevel = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0
        };

        if (!device.Initialize(adapter, deviceArgs))
        {
            goto Error;
        }

        // Create the graphics queue
        if (!queue.Initialize(device))
        {
            goto Error;
        }

        // Set up the swapchain
        var swapChainArgs = new SwapChainCreationArgs
        {
            AllowTearing = true,
            BufferCount = BufferCount,
            Height = window.Height,
            Width = window.Width,
            VSync = true,
            WindowHandle = window.Handle
        };
        if (!surface.Initialize(factory, device, queue, swapChainArgs))
        {
            goto Error;
        }

        // Create the d3d command list
        if (!command.Initialize(device, queue, BufferCount))
        {
            goto Error;
        }

        // Add the resources and register the systems needed
        builder
            .AddResource<RenderData>()
            .AddResource(new D3D12Core
            {
                Factory = factory,
                Surface = surface,
                Command = command,
                Device = device,
                Queue = queue,
                Adapter = adapter
            })


            .AddSystemToStage<BeginFrameSystem>(Stage.PreUpdate, RunCriteria.Always)
            .AddSystemToStage<SwapChainPresentSystem>(Stage.PostUpdate, RunCriteria.Always)
            .AddShutdownSystem<D3D12TearDownSystem>(RunCriteria.Always)
            ;


        


        builder
            .AddModule<D3D12DebugModule>();


        //TestHeap.TestThis(device);

        return true;

Error:
        Logger.Error<D3D12RenderModule1>("Failed to inialize the D3D12 renderer module.");
        device.Shutdown();
        surface.Shutdown();
        queue.Shutdown();
        command.Shutdown();
        adapter.Shutdown();
        factory.Shutdown();

        return false;
    }


    static void EnableDebugLayer()
    {
        // Enable the Debug layer for D3D12
        using ComPtr<ID3D12Debug> spDebugController0 = default;
        using ComPtr<ID3D12Debug1> spDebugController1 = default;
        var hr = D3D12Common.D3D12GetDebugInterface(typeof(ID3D12Debug).GUID, (void**)spDebugController0.GetAddressOf());
        if (Common.FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed {nameof(D3D12Common.D3D12GetDebugInterface)} with HRESULT: {hr}");
            return;
        }

        hr = spDebugController0.Get()->QueryInterface(typeof(ID3D12Debug1).GUID, (void**)spDebugController1.GetAddressOf());
        if (Common.FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed to query {nameof(ID3D12Debug1)} interface with HRESULT: {hr}");
            return;
        }
        spDebugController1.Get()->EnableDebugLayer();
        spDebugController1.Get()->SetEnableGPUBasedValidation(true);
    }
}



public unsafe struct TestHeap
{
    public static void TestThis(ID3D12Device4* device)
    {
        ComPtr<ID3D12Heap> textureHeap = default;
        {
            var desc = new D3D12_HEAP_DESC
            {
                Flags = D3D12_HEAP_FLAGS.D3D12_HEAP_FLAG_ALLOW_ALL_BUFFERS_AND_TEXTURES,
                Alignment = 0,
                Properties = new D3D12_HEAP_PROPERTIES
                {
                    CPUPageProperty = D3D12_CPU_PAGE_PROPERTY.D3D12_CPU_PAGE_PROPERTY_UNKNOWN,
                    CreationNodeMask = 0,
                    MemoryPoolPreference = D3D12_MEMORY_POOL.D3D12_MEMORY_POOL_UNKNOWN,
                    Type = D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_DEFAULT, // this is video memory
                    VisibleNodeMask = 0
                },
                SizeInBytes = MemoryUtils.GigaBytes(1u)
            };
            var hr = device->CreateHeap(&desc, typeof(ID3D12Heap).GUID, (void**)textureHeap.GetAddressOf());
            if (Common.FAILED(hr))
            {
                Logger.Error<TestHeap>($"{nameof(ID3D12Device4.CreateHeap)} for the TextureHeap failed with HRESULT {hr}");
                return;
            }
        }

        ComPtr<ID3D12Heap> uploadHeap = default;
        {
            var desc = new D3D12_HEAP_DESC
            {
                Flags = D3D12_HEAP_FLAGS.D3D12_HEAP_FLAG_ALLOW_ALL_BUFFERS_AND_TEXTURES,
                Alignment = 0,
                Properties = new D3D12_HEAP_PROPERTIES
                {
                    CPUPageProperty = D3D12_CPU_PAGE_PROPERTY.D3D12_CPU_PAGE_PROPERTY_UNKNOWN,
                    CreationNodeMask = 0,
                    MemoryPoolPreference = D3D12_MEMORY_POOL.D3D12_MEMORY_POOL_UNKNOWN,
                    Type = D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_UPLOAD,
                    VisibleNodeMask = 0
                },
                SizeInBytes = MemoryUtils.MegaBytes(128u)
            };
            var hr = device->CreateHeap(&desc, typeof(ID3D12Heap).GUID, (void**)uploadHeap.GetAddressOf());
            if (Common.FAILED(hr))
            {
                Logger.Error<TestHeap>($"{nameof(ID3D12Device4.CreateHeap)} failed with HRESULT {hr}");
                return;
            }
        }

        ComPtr<ID3D12Resource> textureResource = default;
        {
            var resourceDesc = new D3D12_RESOURCE_DESC
            {
                Flags = D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_NONE,
                Width = 1024,
                Height = 1024,
                Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM,
                Alignment = 0,
                DepthOrArraySize = 1,
                Dimension = D3D12_RESOURCE_DIMENSION.D3D12_RESOURCE_DIMENSION_TEXTURE2D,
                Layout = D3D12_TEXTURE_LAYOUT.D3D12_TEXTURE_LAYOUT_UNKNOWN,
                MipLevels = 1,
                SampleDesc = new DXGI_SAMPLE_DESC
                {
                    Quality = 0,
                    Count = 1
                }
            };
            var hr = device->CreatePlacedResource(textureHeap, 0, &resourceDesc, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_ALL_SHADER_RESOURCE, null, typeof(ID3D12Resource).GUID, (void**)textureResource.GetAddressOf());
            if (Common.FAILED(hr))
            {
                Logger.Error<TestHeap>($"{nameof(ID3D12Device4.CreatePlacedResource)} failed with HRESULT {hr}");
                return;
            }
        }

        ComPtr<ID3D12Resource> uploadResource = default;
        {
            ulong totalSize, rowSize = 0ul;
            uint rowCount = 0u;
            D3D12_PLACED_SUBRESOURCE_FOOTPRINT layout = default;

            D3D12_RESOURCE_DESC textureDesc;
            textureResource.Get()->GetDesc(&textureDesc);

            using ComPtr<ID3D12Device4> theDevice = default;
            textureResource.Get()->GetDevice(typeof(ID3D12Device4).GUID, (void**)theDevice.GetAddressOf());

            theDevice.Get()->GetCopyableFootprints(&textureDesc, 0, 1, 0, &layout, &rowCount, &rowSize, &totalSize);

            var uploadDesc = new D3D12_RESOURCE_DESC
            {
                Flags = D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_NONE,
                Height = 1,
                Format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN,
                Alignment = 0,
                Width = totalSize,
                DepthOrArraySize = 1,
                Dimension = D3D12_RESOURCE_DIMENSION.D3D12_RESOURCE_DIMENSION_BUFFER,
                Layout = D3D12_TEXTURE_LAYOUT.D3D12_TEXTURE_LAYOUT_ROW_MAJOR,
                MipLevels = 1,
                SampleDesc = new DXGI_SAMPLE_DESC
                {
                    Count = 1,
                    Quality = 0
                }
            };

            var hr = device->CreatePlacedResource(uploadHeap.Get(), 0, &uploadDesc, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_GENERIC_READ, null, typeof(ID3D12Resource).GUID, (void**)uploadResource.GetAddressOf());
            if (Common.FAILED(hr))
            {
                Logger.Error<TestHeap>($"{nameof(ID3D12Device4.CreatePlacedResource)} failed with HRESULT {hr}");
                return;
            }

            void* a = null;
            uploadResource.Get()->Map(0, null, &a);
            MemoryUtils.Init(a, (uint)totalSize, 1);
            uploadResource.Get()->Unmap(0, null);
        }

        {

            ComPtr<ID3D12CommandQueue> commandQueue = default;
            ComPtr<ID3D12CommandAllocator> allocator = default;
            ComPtr<ID3D12GraphicsCommandList> commandList = default;
            var queueDesc = new D3D12_COMMAND_QUEUE_DESC
            {
                Flags = D3D12_COMMAND_QUEUE_FLAGS.D3D12_COMMAND_QUEUE_FLAG_NONE,
                NodeMask = 0,
                Priority = 0,
                Type = D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_COPY
            };


            var hr = device->CreateCommandQueue(&queueDesc, typeof(ID3D12CommandQueue).GUID, (void**)commandQueue.GetAddressOf());
            hr = device->CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_COPY, typeof(ID3D12CommandAllocator).GUID, (void**)allocator.GetAddressOf());
            hr = device->CreateCommandList1(0, D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_COPY, D3D12_COMMAND_LIST_FLAGS.D3D12_COMMAND_LIST_FLAG_NONE, typeof(ID3D12GraphicsCommandList).GUID, (void**)commandList.GetAddressOf());

            commandList.Get()->Reset(allocator, null);


            {

                var barrier = new D3D12_RESOURCE_BARRIER()
                {
                    Flags = D3D12_RESOURCE_BARRIER_FLAGS.D3D12_RESOURCE_BARRIER_FLAG_BEGIN_ONLY,
                    Transition = new D3D12_RESOURCE_TRANSITION_BARRIER
                    {
                        StateBefore = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE,
                        StateAfter = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_COPY_DEST,
                        Subresource = D3D12Constants.D3D12_RESOURCE_BARRIER_ALL_SUBRESOURCES,
                        pResource = textureResource
                    }
                };
                commandList.Get()->ResourceBarrier(1, &barrier);

                D3D12_TEXTURE_COPY_LOCATION copyDst = new()
                {
                    Type = D3D12_TEXTURE_COPY_TYPE.D3D12_TEXTURE_COPY_TYPE_PLACED_FOOTPRINT,
                    pResource = textureResource.Get()
                };

                D3D12_RESOURCE_DESC textureDesc;
                textureResource.Get()->GetDesc(&textureDesc);


                D3D12_RESOURCE_DESC uploadDesc;
                uploadResource.Get()->GetDesc(&uploadDesc);

                D3D12_TEXTURE_COPY_LOCATION copySrc = new()
                {
                    Type = D3D12_TEXTURE_COPY_TYPE.D3D12_TEXTURE_COPY_TYPE_PLACED_FOOTPRINT,
                    pResource = uploadResource.Get()
                };

                fixed (D3D12_PLACED_SUBRESOURCE_FOOTPRINT* pFootprint = &copyDst.PlacedFootprint)
                {
                    device->GetCopyableFootprints(&textureDesc, 0, 1, 0, pFootprint, null, null, null);
                }

                fixed (D3D12_PLACED_SUBRESOURCE_FOOTPRINT* pFootprint = &copySrc.PlacedFootprint)
                {
                    device->GetCopyableFootprints(&uploadDesc, 0, 1, 0, pFootprint, null, null, null);
                }

                commandList.Get()->CopyTextureRegion(&copySrc, 0, 0, 0, &copyDst, null);
                //commandList.Get()->CopyResource(uploadResource.Get(), textureResource.Get());

                commandList.Get()->Close();

                commandQueue.Get()->ExecuteCommandLists(1, (ID3D12CommandList**)commandList.GetAddressOf());

            }

        }



        static int Align256(int width, int numBytesPerPixel, int height)
        {
            return ((width * numBytesPerPixel + 255) & ~255) * (height - 1) + width * numBytesPerPixel;
        }
    }
}
