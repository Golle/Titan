using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Rendering.Commands
{
    [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 2 + sizeof(long) * 6)]
    internal unsafe struct SetVertexShaderSamplersCommand
    {
        internal RenderCommandTypes Type;
        internal uint NumberOfSamplers;
        private long _fixedBuffer;
        internal ID3D11SamplerState** Samplers
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (ID3D11SamplerState**)Unsafe.AsPointer(ref _fixedBuffer);
        }

        public SetVertexShaderSamplersCommand(uint numberOfSamplers)
        {
            Type = RenderCommandTypes.SetVertexShaderSamplers;
            NumberOfSamplers = numberOfSamplers;
            _fixedBuffer = 0;
        }
    }
}
