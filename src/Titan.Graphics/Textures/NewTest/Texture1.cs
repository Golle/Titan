using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Graphics.Textures.NewTest
{
    public readonly struct Texture1
    {
        internal readonly ShaderResourceView View;
        internal readonly Texture2D Texture2D;
        internal Texture1(in ShaderResourceView view, in Texture2D texture2D)
        {
            View = view;
            Texture2D = texture2D;
        }
    }
}
