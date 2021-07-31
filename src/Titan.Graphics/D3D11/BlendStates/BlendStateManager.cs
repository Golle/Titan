using System;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.D3D11.Buffers;
using Titan.Windows.D3D11;
using static Titan.Windows.Common;

namespace Titan.Graphics.D3D11.BlendStates
{
    public record BlendStateCreation(byte RenderTargetIndex = 0)
    {
        public float[] BlendFactor { get; init; }
        public uint SampleMask { get; init; } = 0xffffffff;
        public D3D11_BLEND SourceBlend { get; init; } = D3D11_BLEND.D3D11_BLEND_SRC_ALPHA;
        public D3D11_BLEND DestinationBlend { get; init; } = D3D11_BLEND.D3D11_BLEND_INV_SRC_ALPHA;
        public D3D11_BLEND_OP BlendOperation { get; init; } = D3D11_BLEND_OP.D3D11_BLEND_OP_ADD;
        public D3D11_BLEND SourceBlendAlpha { get; init; } = D3D11_BLEND.D3D11_BLEND_ONE;
        public D3D11_BLEND DestinationBlendAlpha { get; init; } = D3D11_BLEND.D3D11_BLEND_ZERO;
        public D3D11_BLEND_OP BlendOperationAlpha { get; init; } = D3D11_BLEND_OP.D3D11_BLEND_OP_ADD;
        public D3D11_COLOR_WRITE_ENABLE RenderTargetWriteMask { get; init; } = D3D11_COLOR_WRITE_ENABLE.D3D11_COLOR_WRITE_ENABLE_ALL;
    }

    public unsafe class BlendStateManager : IDisposable
    {
        private readonly ID3D11Device* _device;
        private ResourcePool<BlendState> _resourcePool;
        
        private const uint MaxBuffers = 16u;
        internal BlendStateManager(ID3D11Device* device)
        {
            Logger.Trace<BlendStateManager>($"Init with {MaxBuffers} slots");
            _resourcePool.Init(MaxBuffers);
            _device = device;
        }

        public Handle<BlendState> Create(BlendStateCreation args)
        {
            Logger.Trace<BlendState>($"Create BlendState with DATA");
            var handle = _resourcePool.CreateResource();
            if (!handle.IsValid())
            {
                throw new InvalidOperationException("Failed to Create a BlendState Handle");
            }

            if (args.RenderTargetIndex > 7)
            {
                throw new ArgumentOutOfRangeException($"{nameof(args.RenderTargetIndex)} must be between 0 and 7");
            }

            if (args.BlendFactor != null && args.BlendFactor.Length != 4)
            {
                throw new InvalidOperationException("BlendFactor must be null or of length 4");
            }

            D3D11_BLEND_DESC desc = default;
            ref var renderTarget = ref desc.RenderTarget[args.RenderTargetIndex]; // TODO: add support for multiple render targets
            renderTarget.BlendEnable = true;
            renderTarget.SrcBlend = args.SourceBlend;
            renderTarget.DestBlend = args.DestinationBlend;
            renderTarget.BlendOp = args.BlendOperation;
            renderTarget.SrcBlendAlpha = args.SourceBlendAlpha;
            renderTarget.DestBlendAlpha = args.DestinationBlendAlpha;
            renderTarget.BlendOpAlpha = args.BlendOperationAlpha;
            renderTarget.RenderTargetWriteMask = args.RenderTargetWriteMask;


            renderTarget.RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE.D3D11_COLOR_WRITE_ENABLE_ALL;

            var blendState = _resourcePool.GetResourcePointer(handle);
            blendState->SampleMask = args.SampleMask;
            if (args.BlendFactor != null)
            {
                fixed (float* pBlendFactor = args.BlendFactor)
                {
                    const int size = sizeof(float)*4;
                    System.Buffer.MemoryCopy(pBlendFactor, blendState->BlendFactor, size, size);
                }
            }
            else
            {
                for (var i = 0; i < 4; ++i)
                {
                    blendState->BlendFactor[i] = 1.0f;
                }
            }
            CheckAndThrow(_device->CreateBlendState(&desc, &blendState->State), nameof(ID3D11Device.CreateBlendState));

            return handle;
        }


        public void Release(in Handle<BlendState> handle)
        {
            Logger.Trace<BlendStateManager>($"Releasing blend state with handle {handle}");
            ReleaseInternal(handle);
            _resourcePool.ReleaseResource(handle);
        }

        private void ReleaseInternal(Handle<BlendState> handle)
        {
            var blendState = _resourcePool.GetResourcePointer(handle);
            if (blendState->State != null)
            {
                blendState->State->Release();
            }
            *blendState = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref readonly BlendState Access(in Handle<BlendState> handle) => ref _resourcePool.GetResourceReference(handle);

        public void Dispose()
        {
            foreach (var handle in _resourcePool.EnumerateUsedResources())
            {
                Logger.Warning<BlendStateManager>($"Releasing an unreleased BlendState with handle {handle.Value}");
                ReleaseInternal(handle);
            }
            Logger.Trace<BufferManager>("Terminate resource pool");
            _resourcePool.Terminate();
        }
    }
}
