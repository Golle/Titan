using System;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Windows;
using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11.Rasterizer
{
    public record RasterizerStateCreation(CullMode CullMode = CullMode.None, FillMode FillMode = FillMode.Solid);

    public unsafe class RasterizerManager : IDisposable
    {

        private readonly ID3D11Device* _device;
        private ResourcePool<RasterizerState> _resourcePool;
        private const uint MaxStates = 32u;
        internal RasterizerManager(ID3D11Device* device)
        {
            Logger.Trace<RasterizerManager>($"Init with {MaxStates} slots");
            _device = device;

            _resourcePool.Init(MaxStates);
        }


        public Handle<RasterizerState> Create(RasterizerStateCreation args)
        {
            var handle = _resourcePool.CreateResource();
            if (!handle.IsValid())
            {
                throw new InvalidOperationException($"Failed to Create a {nameof(RasterizerState)} Handle");
            }
            Logger.Trace<RasterizerManager>($"Creating {nameof(RasterizerState)} with .....");

            var desc = new D3D11_RASTERIZER_DESC
            {
                CullMode = (D3D11_CULL_MODE)args.CullMode,
                FillMode = (D3D11_FILL_MODE)args.FillMode,
                DepthClipEnable = 1,
            };

            var state = _resourcePool.GetResourcePointer(handle);
            state->CullMode = args.CullMode;
            state->FillMode = args.FillMode;

            Common.CheckAndThrow(_device->CreateRasterizerState(&desc, &state->State), nameof(ID3D11Device.CreateRasterizerState));
            
            return handle;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref readonly RasterizerState Access(in Handle<RasterizerState> handle) => ref _resourcePool.GetResourceReference(handle);

        internal void Release(in Handle<RasterizerState> handle)
        {
            ReleaseInternal(handle);
            _resourcePool.ReleaseResource(handle);
        }

        private void ReleaseInternal(in Handle<RasterizerState> handle)
        {
            var rasterizerState = _resourcePool.GetResourcePointer(handle);
            if (rasterizerState->State != null)
            {
                rasterizerState->State->Release();
            }
        }

        public void Dispose()
        {
            foreach (var handle in _resourcePool.EnumerateUsedResources())
            {
                Logger.Warning<RasterizerManager>($"Releasing an unreleased RasterizerState with handle {handle.Value}");
                ReleaseInternal(handle);
            }
            Logger.Trace<RasterizerManager>("Terminate resource pool");
            _resourcePool.Terminate();
        }
    }
}
