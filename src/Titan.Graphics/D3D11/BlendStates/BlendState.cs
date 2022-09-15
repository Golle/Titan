using Titan.Platform.Win32.D3D11;

namespace Titan.Graphics.D3D11.BlendStates
{
    public unsafe struct BlendState
    {
        public ID3D11BlendState* State;
        public fixed float BlendFactor[4];
        public uint SampleMask;
    }
}
