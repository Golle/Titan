using System.Collections.Generic;
using System.Threading;
using Titan.Core.Memory;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.States
{
    internal unsafe class SamplerStateManager : ISamplerStateManager
    {
        private readonly IDictionary<D3D11_SAMPLER_DESC, SamplerStateHandle> _cachedHandles = new Dictionary<D3D11_SAMPLER_DESC, SamplerStateHandle>();

        private ComPtr<ID3D11Device> _device;
        private readonly SamplerState* _samplers;
        private uint _maxSamplers;
        private int _numberOfSamplers;

        public SamplerStateManager(ID3D11Device* device, IMemoryManager memoryManager)
        {
            _device = new ComPtr<ID3D11Device>(device);
            var memory = memoryManager.GetMemoryChunkValidated<SamplerState>("SamplerState");
            _samplers = memory.Pointer;
            _maxSamplers = memory.Count;
        }
        public SamplerStateHandle GetOrCreate(D3D11_FILTER filter, D3D11_TEXTURE_ADDRESS_MODE addressU, D3D11_TEXTURE_ADDRESS_MODE addressV, D3D11_TEXTURE_ADDRESS_MODE addressW, D3D11_COMPARISON_FUNC comparisonFunc)
        {
            var desc = new D3D11_SAMPLER_DESC
            {
                Filter = filter,
                AddressU = addressU,
                AddressV = addressV,
                AddressW = addressW,
                ComparisonFunc = comparisonFunc
            };
            
            if (!_cachedHandles.TryGetValue(desc, out var handle))
            {
                handle = Interlocked.Increment(ref _numberOfSamplers) - 1;
                Common.CheckAndThrow(_device.Get()->CreateSamplerState(&desc, &_samplers[handle].Pointer), "CreateSamplerState");
                _cachedHandles.Add(desc, handle);
            }

            return handle;
        }

        public ref readonly SamplerState this[in SamplerStateHandle handle] => ref _samplers[handle];

        public void Dispose()
        {
            for (var i = 1; i < _numberOfSamplers; ++i)
            {
                ref var state = ref _samplers[i];
                if (state.Pointer != null)
                {
                    state.Pointer->Release();
                    state.Pointer = null;
                }
            }

            _numberOfSamplers = 0;
            _device.Dispose();
        }
    }
}
