using System.Runtime.InteropServices;
using Titan.Graphics.Resources;

namespace Titan.Graphics.Materials
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Texture
    {
        public ShaderResourceViewHandle Handle;
        public TextureHandle TextureHandle;
    }
}
