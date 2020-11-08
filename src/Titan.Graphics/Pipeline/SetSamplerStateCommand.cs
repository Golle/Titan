using System.Runtime.InteropServices;
using Titan.Graphics.States;

namespace Titan.Graphics.Pipeline
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SetSamplerStateCommand
    {
        public SamplerStateHandle Sampler;
        public uint Slot;
    }
}
