using System;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    internal interface IIndexBufferManager : IDisposable
    {
        unsafe IndexBufferHandle CreateIndexBuffer<T>(uint count, void* initialData = null, D3D11_USAGE usage = default, D3D11_CPU_ACCESS_FLAG cpuAccess = default, D3D11_RESOURCE_MISC_FLAG miscFlags = default) where T : unmanaged;
        void DestroyBuffer(in IndexBufferHandle handle);
        ref readonly IndexBuffer this[in IndexBufferHandle handle] { get; }
    }
}
