using System;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11
{

    public unsafe class Texture2D : IDisposable
    {

        public Texture2D(IGraphicsDevice device)
        {
            D3D11_TEXTURE2D_DESC desc;

            //desc.
            //device.Ptr->CreateTexture2D()
        }

        public void Dispose()
        {
        }
    }

    public unsafe class DepthStencilView : IDisposable
    {

        public DepthStencilView(IGraphicsDevice device)
        {
            
        }
        public void Dispose()
        {
        }
    }
}
