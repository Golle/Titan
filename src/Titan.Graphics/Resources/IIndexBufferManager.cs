using System;
using Titan.Core.Common;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    public interface IIndexBufferManager : IDisposable
    {
        void Initialize(IGraphicsDevice graphicsDevice);
        unsafe Handle<IndexBuffer> CreateIndexBuffer<T>(uint count, void* initialData = null, D3D11_USAGE usage = default, D3D11_CPU_ACCESS_FLAG cpuAccess = default, D3D11_RESOURCE_MISC_FLAG miscFlags = default) where T : unmanaged;
        void DestroyBuffer(in Handle<IndexBuffer> handle);
        ref readonly IndexBuffer this[in Handle<IndexBuffer> handle] { get; }
    }
}
