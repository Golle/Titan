using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core.Common;
using Titan.Graphics.Resources;

namespace Titan.Graphics.Textures
{
    [StructLayout(LayoutKind.Sequential)]
    [SkipLocalsInit]
    public readonly struct Texture
    {
        public readonly Handle<ShaderResourceView> Resource;
        public readonly Handle<Texture2D> Texture2D;
        public Texture(in Handle<ShaderResourceView> resource, in Handle<Texture2D> texture2d)
        {
            Resource = resource;
            Texture2D = texture2d;
        }
    }
}
