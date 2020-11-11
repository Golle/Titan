using System.Runtime.InteropServices;
using Titan.Graphics.D3D11;

namespace Titan.Graphics.Materials
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)] //TOOD: remove pack 1 if we don't have 4 bools
    public struct Material
    {
        public NormalMap NormalMap;

        public Color Diffuse;
        public Color Ambient;
        public Color Specular;
        public Color Emissive;

        public bool IsTextured;
        public bool HasNormalMap;
        public bool IsTransparent;
        private bool NOOP;
    }
}
