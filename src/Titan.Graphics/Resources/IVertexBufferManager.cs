using System;
using Titan.Core.Common;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    public interface IVertexBufferManager : IDisposable
    {
        void Initialize(IGraphicsDevice graphicsDevice);
        unsafe Handle<VertexBuffer> CreateVertexBuffer(uint count, uint stride, void* initialData = null, D3D11_USAGE usage = default, D3D11_CPU_ACCESS_FLAG cpuAccess = default, D3D11_RESOURCE_MISC_FLAG miscFlags = default);
        void DestroyBuffer(in Handle<VertexBuffer> handle);
        ref readonly VertexBuffer this[in Handle<VertexBuffer> handle] { get; }
    }
}
