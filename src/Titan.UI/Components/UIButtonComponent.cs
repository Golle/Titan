using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Graphics.D3D11.Textures;

namespace Titan.UI.Components
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size=4)]
    public struct UIButtonComponent
    {

    }

    public struct UISprite
    {
        public Handle<Texture> Texture;
    }
}
