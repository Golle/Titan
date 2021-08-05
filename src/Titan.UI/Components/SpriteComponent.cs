using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Graphics.D3D11.Textures;

namespace Titan.UI.Components
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SpriteComponent
    {
        public Handle<Texture> Texture;
    }
}
