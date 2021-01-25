using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.States
{
    internal unsafe class DepthStencilStateManager : IDepthStencilStateManager
    {
        private readonly IMemoryManager _memoryManager;
        private readonly IDictionary<D3D11_DEPTH_STENCIL_DESC, DepthStencilStateHandle> _cachedHandles = new Dictionary<D3D11_DEPTH_STENCIL_DESC, DepthStencilStateHandle>();

        private ComPtr<ID3D11Device> _device;
        private DepthStencilState* _states;
        private uint _maxStates;

        private int _numberOfStates;
        public DepthStencilStateManager(IMemoryManager memoryManager)
        {
            _memoryManager = memoryManager;
        }

        public void Initialize(IGraphicsDevice graphicsDevice)
        {
            _device = graphicsDevice is D3D11GraphicsDevice device ? new ComPtr<ID3D11Device>(device.Ptr) : throw new ArgumentException($"Trying to initialize a D3D11 {nameof(DepthStencilStateManager)} with the wrong device.", nameof(graphicsDevice));
            var memory = _memoryManager.GetMemoryChunkValidated<DepthStencilState>("DepthStencilState");
            _states = memory.Pointer;
            _maxStates = memory.Count;
        }

        public DepthStencilStateHandle GetOrCreate(D3D11_DEPTH_WRITE_MASK writeMask, D3D11_COMPARISON_FUNC comparisonFunc)
        {
            var desc = new D3D11_DEPTH_STENCIL_DESC
            {
                DepthEnable = 1,
                DepthWriteMask = D3D11_DEPTH_WRITE_MASK.D3D11_DEPTH_WRITE_MASK_ALL,
                DepthFunc = D3D11_COMPARISON_FUNC.D3D11_COMPARISON_LESS_EQUAL
            };
            
            if (!_cachedHandles.TryGetValue(desc, out var handle))
            {
                handle = Interlocked.Increment(ref _numberOfStates) - 1;
                Common.CheckAndThrow(_device.Get()->CreateDepthStencilState(&desc, &_states[handle].Pointer), "CreateDepthStencilState");
                _cachedHandles.Add(desc, handle);
            }
            return handle;
        }

        public ref readonly DepthStencilState this[in DepthStencilStateHandle handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _states[handle];
        }

        public void Dispose()
        {
            if (_states != null)
            {
                for (var i = 0; i < _numberOfStates; ++i)
                {
                    ref var state = ref _states[i];
                    if (state.Pointer != null)
                    {
                        state.Pointer->Release();
                        state.Pointer = null;
                    }
                }
                _numberOfStates = 0;
                _states = null;
                _device.Dispose();
            }
        }
    }
}
