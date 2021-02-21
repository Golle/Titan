using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Titan.Core.Common;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11.Samplers
{
    internal unsafe class SamplerManager : IDisposable
    {
        private readonly ID3D11Device* _device;
        private ResourcePool<Sampler> _resourcePool;


        private List<Handle<Sampler>> _usedHandles = new();
        internal SamplerManager(Device device)
        {
            _device = device.Get();
            _resourcePool.Init(32);
        }

        internal Handle<Sampler> Create(SamplerCreation args)
        {
            var handle = _resourcePool.CreateResource();
            if (!handle.IsValid())
            {
                throw new InvalidOperationException("Failed to Create a Sampler Handle");
            }

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

            _usedHandles.Add(handle);
            return handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref readonly Sampler Access(in Handle<Sampler> handle) => ref _resourcePool.GetResourceReference(handle);

        internal void Release(in Handle<Sampler> handle)
        {
            ReleaseInternal(handle);
            
            _usedHandles.Remove(handle);
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
            foreach (var handle in _usedHandles)
            {
                ReleaseInternal(handle);
            }   
            _resourcePool.Terminate();
        }
    }
}
