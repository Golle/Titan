using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Maths;
using Titan.Core.Memory;
using Titan.Graphics.D3D12.Memory;
using Titan.Graphics.D3D12.Utils;
using Titan.Graphics.Resources;
using Titan.Platform.Win32.D3D12;
using Titan.Setup;
using Titan.Setup.Configs;

namespace Titan.Graphics.Rendering.Sprites;

public record SpriteRenderingConfig(TextureFormat RenderTargetFormat) : IConfiguration, IDefault<SpriteRenderingConfig>
{
    public static SpriteRenderingConfig Default => new(TextureFormat.RGBA8U);
}

[SkipLocalsInit]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal struct RenderCamera
{
    public SizeF Size;
    public Vector2 Position;
    public Vector2 Scale;
}

[SkipLocalsInit]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal struct SpriteInstanceData3
{
    public Vector2 Offset;
    public Vector2 TextureSize;
    public Vector2 Pivot;
    public Vector2 Scale;
    public Color Color;
    public RectangleF DrawRect;        //Type RectangleF in C#. struct {X,Y, Width, Height} 
    public Vector2 SinCosRotation;
    public uint TextureId;
}

[SkipLocalsInit]
file unsafe struct PerDrawData
{
    public static readonly uint Size = (uint)sizeof(PerDrawData);
    public RenderCamera Camera;
    public uint BufferIndex;
    public uint LinearSampling;
}

file struct RootSignatureIndex
{
    public const uint SRVTable = 0;
    public const uint PerFrameData = 1;// currently not used
    public const uint PerDrawData = 2;
}
internal unsafe class D3D12SpriteRenderer
{
    private Handle<GPUBuffer> _indexBuffer;
    private Handle<RootSignature> _signature;
    private Handle<PipelineState> _alphaBlendPipelineState;
    private Handle<PipelineState> _noBlendPipelineState;

    private IResourceManager _resourceManager;
    private D3D12Allocator _allocator;

    public bool Init(IResourceManager resourceManager, D3D12Allocator allocator, SpriteRenderingConfig config, in SpriteRendererAssets assets)
    {
        _indexBuffer = CreateIndexBuffer(resourceManager);
        if (_indexBuffer.IsInvalid)
        {
            Logger.Error<D3D12SpriteRenderer>("Failed to create the IndexBuffer");
            return false;
        }

        _signature = CreateRootSignature(resourceManager);
        if (_signature.IsInvalid)
        {
            Logger.Error<D3D12SpriteRenderer>("Failed to create the RootSignature.");
            return false;
        }

        _noBlendPipelineState = CreatePipelineState(resourceManager, config.RenderTargetFormat, assets, BlendStateType.Disabled);
        if (_noBlendPipelineState.IsInvalid)
        {
            Logger.Error<D3D12SpriteRenderer>("Failed to create the NoBlendPipelineState");
            return false;
        }

        _alphaBlendPipelineState = CreatePipelineState(resourceManager, config.RenderTargetFormat, assets, BlendStateType.AlphaBlend);
        if (_alphaBlendPipelineState.IsInvalid)
        {
            Logger.Error<D3D12SpriteRenderer>("Failed to create the AlphaBlendPipelineState");
            return false;
        }

        _resourceManager = resourceManager;
        _allocator = allocator;
        return true;
    }

    private Handle<PipelineState> CreatePipelineState(IResourceManager resourceManager, TextureFormat format, in SpriteRendererAssets assets, BlendStateType blendState)
    {
        var state = resourceManager.CreatePipelineState(new CreatePipelineStateArgs
        {
            PixelShader = assets.PixelShader,
            VertexShader = assets.VertexShader,
            RenderTargetFormats = new[] { format },
            RootSignature = _signature,
            PrimitiveTopology = PrimitiveTopologyType.Triangle,
            BlendState = blendState
        });

        return state;
    }

    private static Handle<GPUBuffer> CreateIndexBuffer(IResourceManager resourceManager)
    {
        var indices = stackalloc ushort[6];
        D3D12Helpers.InitSquareIndexBuffer(indices, 6);
        var buffer = resourceManager.CreateBuffer(new CreateBufferArgs(6, sizeof(ushort), true, false, BufferType.IndexBuffer, new TitanBuffer(indices, sizeof(ushort) * 6)));
        return buffer;
    }

    private static Handle<RootSignature> CreateRootSignature(IResourceManager resourceManager)
    {
        // set up the root signature
        const int rangeCount = 6;
        var ranges = stackalloc D3D12_DESCRIPTOR_RANGE1[rangeCount];
        D3D12Helpers.InitDescriptorRanges(new Span<D3D12_DESCRIPTOR_RANGE1>(ranges, rangeCount), D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_SRV);
        var signature = resourceManager.CreateRootSignature(new CreateRootSignatureArgs
        {
            Flags = D3D12_ROOT_SIGNATURE_FLAGS.D3D12_ROOT_SIGNATURE_FLAG_NONE,
            Parameters = new[]
            {
                CD3DX12_ROOT_PARAMETER1.AsDescriptorTable(rangeCount, ranges),
                CD3DX12_ROOT_PARAMETER1.AsConstantBufferView(registerSpace: 1, shaderRegister: 0),
                CD3DX12_ROOT_PARAMETER1.AsConstantBufferView(registerSpace: 1, shaderRegister: 1)
            },
            StaticSamplers = new[]
            {
                D3D12Helpers.CreateStaticSamplerDesc(SampleState.Linear, 0, 0),
                D3D12Helpers.CreateStaticSamplerDesc(SampleState.Point, 1, 0)
            }
        });
        return signature;
    }

    public void Begin(D3D12CommandList commandList)
    {
        commandList.SetRootSignature(_signature);

        commandList.SetIndexBuffer(_indexBuffer);
        commandList.SetPrimitiveTopology(PrimitiveTopology.TriangleList);
    }

    public void Render(in D3D12CommandList commandList, in RenderCamera camera, in TitanArray<SpriteInstanceData3> instances, bool alphaBlend)
    {
        commandList.SetPipelineState(alphaBlend ? _alphaBlendPipelineState : _noBlendPipelineState);

        var spriteData = _allocator.AllocateTempStructuredBuffer<SpriteInstanceData3>(instances.Length, true);
        MemoryUtils.Copy(spriteData.CPUAddress, instances.GetPointer(), sizeof(SpriteInstanceData3) * instances.Length);
        var drawDataBuffer = _allocator.AllocateTempConstantBuffer(PerDrawData.Size);
        
        var drawData = new PerDrawData
        {
            BufferIndex = spriteData.DescriptorIndex,
            LinearSampling = 0,
            Camera = camera
        };
        MemoryUtils.Copy(drawDataBuffer.CPUAddress, &drawData, PerDrawData.Size);
        commandList.SetGraphicsRootConstantBufferView(RootSignatureIndex.PerDrawData, drawDataBuffer.GPUAddress);
        commandList.DrawIndexInstanced(6, (uint)instances.Length);
    }

    public void End(in D3D12CommandList commandList)
    {
        // noop
    }

    public void Shutdown()
    {
        _resourceManager.DestroyBuffer(_indexBuffer);
        _resourceManager.DestroyRootSignature(_signature);
        _resourceManager.DestroyPipelineState(_alphaBlendPipelineState);
    }
}
