using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Titan.Core.Logging;

namespace Titan.Core.Memory
{
    public unsafe class MemoryManager : IMemoryManager
    {
        private readonly IDictionary<string, MemoryChunk> _memory = new Dictionary<string, MemoryChunk>();
        private void* _memoryChunks;

        public void Initialize(in ChunkDescriptor[] descriptors)
        {
            if (_memoryChunks != null)
            {
                throw new InvalidOperationException("Memory pool has already been initialized");
            }

            var size = (int)descriptors.Sum(d => d.Count * d.Size);
            LOGGER.Debug("Allocating {0} bytes of memory", size);
            _memoryChunks = (void*)Marshal.AllocHGlobal(size);
            if (_memoryChunks == null)
            {
                throw new OutOfMemoryException($"Failed to allocate {size} bytes of memory.");
            }

            var lastOffset = (byte*)_memoryChunks;
            foreach (var descriptor in descriptors)
            {
                LOGGER.Debug("Memory: identifier {0} with stride {1} and count {2}", descriptor.Identifier, descriptor.Size, descriptor.Count);
                _memory.Add(descriptor.Identifier, new MemoryChunk(lastOffset, descriptor.Size, descriptor.Count));
                lastOffset += descriptor.Count * descriptor.Size;
            }
        }

        public MemoryChunk GetMemoryChunk(string identifier)
        {
            Debug.Assert(_memoryChunks != null, "MemoryManager has not been initialized.");
            return _memory[identifier];
        }

        public MemoryChunk<T> GetMemoryChunkValidated<T>(string identifier) where T : unmanaged
        {
            Debug.Assert(_memoryChunks != null, "MemoryManager has not been initialized.");
            
            var chunk = _memory[identifier];
            if (sizeof(T) != chunk.Stride)
            {
                throw new InvalidOperationException($"The stride of the memory chunk is {chunk.Stride} but the size of {typeof(T)} is {sizeof(T)}");
            }
            return new MemoryChunk<T>(chunk); ;
        }

        public void Dispose()
        {
            if (_memoryChunks != null)
            {
                Marshal.FreeHGlobal((nint)_memoryChunks);
                _memoryChunks = null;
                _memory.Clear();
            }
        }
    }
}
