using System;
using Titan.Graphics.D3D11;

namespace Titan.Graphics
{
    public interface IGraphicsDevice : IDisposable
    {
        void Initialize(uint refreshRate, bool debug = false);
        IRenderContext ImmediateContext { get; }
        public void ResizeBuffers();
    }
}
