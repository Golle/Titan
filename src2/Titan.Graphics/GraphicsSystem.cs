using System;
using System.Diagnostics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Pipeline;
using Titan.Graphics.D3D11.Textures;
using Titan.Windows.D3D11;

namespace Titan.Graphics
{
    public class GraphicsSystem : IDisposable
    {

        private GraphicsSystem()
        {
            
        }

        public static GraphicsSystem Create()
        {


            return new GraphicsSystem();
        }

        public void Dispose()
        {

        }
    }
}
