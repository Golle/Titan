using System;
using Titan.Core.Common;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    public interface IConstantBufferManager : IDisposable
    {
        Handle<ConstantBuffer> CreateConstantBuffer<T>(in T data = default, D3D11_USAGE usage = default, D3D11_CPU_ACCESS_FLAG cpuAccess = default, D3D11_RESOURCE_MISC_FLAG miscFlags = default) where T : unmanaged;
        void DestroyBuffer(in Handle<ConstantBuffer> handle);
        ref readonly ConstantBuffer this[in Handle<ConstantBuffer> handle] { get; }
        void Initialize(IGraphicsDevice graphicsDevice);
    }
}
