using System.Runtime.InteropServices;
using Titan.Graphics.Resources;

namespace Titan.Graphics.Materials
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NormalMap
    {
        public ShaderResourceViewHandle Handle;
        public TextureHandle TextureHandle;
    }
}
