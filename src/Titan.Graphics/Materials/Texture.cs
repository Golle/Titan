using System.Runtime.InteropServices;
using Titan.Graphics.Resources;

namespace Titan.Graphics.Materials
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Texture
    {
        public Handle<ShaderResourceView> Handle;
        public Handle<Resources.Texture> TextureHandle;
    }
}
