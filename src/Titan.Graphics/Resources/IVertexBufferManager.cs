using System;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    internal interface IVertexBufferManager : IDisposable
    {
        unsafe VertexBufferHandle CreateVertexBuffer(uint count, uint stride, void* initialData = null, D3D11_USAGE usage = default, D3D11_CPU_ACCESS_FLAG cpuAccess = default, D3D11_RESOURCE_MISC_FLAG miscFlags = default);
        void DestroyBuffer(in VertexBufferHandle handle);
        ref readonly VertexBuffer this[in VertexBufferHandle handle] { get; }
    }
}
