using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;
using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11.Pipeline
{
    public struct Pipeline
    {
        public bool ClearRenderTargets;
        public bool ClearDepthStencil;


        public Handle<Buffer> IndexBuffer;
        public Handle<Buffer> VertexBuffer;
        public Handle<PixelShader> PixelShader;
        public Handle<VertexShader> VertexShader;


        public Color ClearColor;

        public Handle<Texture>[] PixelShaderResources; // PS Input
        public Handle<Texture>[] VertexShaderResources; // VS Input
        public Handle<Texture>[] RenderTargets; // Output

        
        

    }
    
}
