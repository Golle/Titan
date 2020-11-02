using System.Runtime.InteropServices;
using Titan.Graphics.D3D11.State;

namespace Titan.Graphics.Pipeline
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SetSamplerStateCommand
    {
        public SamplerState Sampler;
        public uint Slot;
    }
}
