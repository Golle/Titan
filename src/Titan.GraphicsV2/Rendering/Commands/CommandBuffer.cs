using System;
using System.Runtime.InteropServices;

namespace Titan.GraphicsV2.Rendering.Commands
{
    internal unsafe class CommandBuffer : IDisposable
    {
        private readonly int _size;
        private byte* _commands;
        public CommandBuffer(byte[] commandBuffer, int size)
        {
            _size = size;
            _commands = (byte*)Marshal.AllocHGlobal(size);
            fixed (byte* pBuffer = commandBuffer)
            {
                Buffer.MemoryCopy(pBuffer, _commands, size, size);
            }
        }

        internal CommandBufferEnumerator Enumerate() => new(_commands, _size);
        public void Dispose()
        {
            if(_commands != null)
            {
                Marshal.FreeHGlobal((nint)_commands);
                _commands = null;
            }
        }
    }
}