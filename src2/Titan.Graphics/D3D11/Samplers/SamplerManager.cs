using System;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Windows;
using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11.Samplers
{
    public unsafe class SamplerManager : IDisposable
    {
        private readonly ID3D11Device* _device;
        private ResourcePool<Sampler> _resourcePool;
        private const uint MaxSamplers = 32u;
        internal SamplerManager(ID3D11Device* device)
        {
            Logger.Trace<SamplerManager>($"Init with {MaxSamplers} slots");
            _device = device;
            
            _resourcePool.Init(MaxSamplers);
            
        }

        internal Handle<Sampler> Create(SamplerCreation args)
        {
            var handle = _resourcePool.CreateResource();
            if (!handle.IsValid())
            {
                throw new InvalidOperationException("Failed to Create a Sampler Handle");
            }
            Logger.Trace<SamplerManager>($"Creating sampler with filter {args.Filter} and comparison func {args.ComparisonFunc}");

            var sampler = _resourcePool.GetResourcePointer(handle);
            sampler->Handle = handle;
            sampler->Filter = args.Filter;
            sampler->AddressU = args.AddressU;
            sampler->AddressV = args.AddressV;
            sampler->AddressW = args.AddressW;
            sampler->ComparisonFunc = args.ComparisonFunc;

            var desc = new D3D11_SAMPLER_DESC
            {
                Filter = (D3D11_FILTER) args.Filter,
                AddressU = (D3D11_TEXTURE_ADDRESS_MODE) args.AddressU,
                AddressV = (D3D11_TEXTURE_ADDRESS_MODE) args.AddressV,
                AddressW = (D3D11_TEXTURE_ADDRESS_MODE) args.AddressW,
                ComparisonFunc = (D3D11_COMPARISON_FUNC) args.ComparisonFunc
            };
            
            Common.CheckAndThrow(_device->CreateSamplerState(&desc, &sampler->SamplerState), nameof(ID3D11Device.CreateSamplerState));
            
            return handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref readonly Sampler Access(in Handle<Sampler> handle) => ref _resourcePool.GetResourceReference(handle);

        internal void Release(in Handle<Sampler> handle)
        {
            ReleaseInternal(handle);
            _resourcePool.ReleaseResource(handle);
        }

        private void ReleaseInternal(in Handle<Sampler> handle)
        {
            var sampler = _resourcePool.GetResourcePointer(handle);
            if (sampler->SamplerState != null)
            {
                sampler->SamplerState->Release();
            }
        }

        public void Dispose()
        {
            foreach (var handle in _resourcePool.EnumerateUsedResources())
            {
                Logger.Warning<SamplerManager>($"Releasing an unreleased Aampler with handle {handle.Value}");
                ReleaseInternal(handle);
            }
            Logger.Trace<SamplerManager>("Terminate resource pool");
            _resourcePool.Terminate();
        }
    }
}
