using System;
using Titan.Graphics.D3D11;

namespace Titan.Graphics.Pipeline.Renderers
{
    internal interface IRenderer : IDisposable
    {
        void Render(IRenderContext context);
        
    }
}
