using System;
using System.IO;

namespace Titan.GraphicsV2.Rendering.Commands
{
    internal class CommandBufferBuilder : IDisposable
    {
        private readonly MemoryStream _stream = new();
        internal unsafe void Write<T>(in T command) where T : unmanaged
        {
            fixed (T* pCommand = &command)
            {
                _stream.Write(new ReadOnlySpan<byte>(pCommand, sizeof(T)));
            }
        }

        public CommandBuffer Build() => new(_stream.GetBuffer(), (int) _stream.Length);
        public void Dispose() => _stream.Dispose();
    }
}
