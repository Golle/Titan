using System;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.States
{
    public interface IDepthStencilStateManager : IDisposable
    {
        void Initialize(IGraphicsDevice graphicsDevice);
        DepthStencilStateHandle GetOrCreate(D3D11_DEPTH_WRITE_MASK writeMask = D3D11_DEPTH_WRITE_MASK.D3D11_DEPTH_WRITE_MASK_ALL, D3D11_COMPARISON_FUNC comparisonFunc = D3D11_COMPARISON_FUNC.D3D11_COMPARISON_LESS_EQUAL);
        ref readonly DepthStencilState this[in DepthStencilStateHandle handle] { get; }
    }
}
