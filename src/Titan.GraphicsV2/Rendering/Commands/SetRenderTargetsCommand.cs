using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Rendering.Commands
{
    [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 2 + sizeof(long) * 6)]
    internal unsafe struct SetRenderTargetsCommand
    {
        internal RenderCommandTypes Type;
        internal uint NumberOfTargets;
        private long _fixedBuffer;
        internal ID3D11RenderTargetView** RenderTargets => (ID3D11RenderTargetView**) Unsafe.AsPointer(ref _fixedBuffer);
        public SetRenderTargetsCommand(uint numberOfTargets)
        {
            Type = RenderCommandTypes.SetRenderTarget;
            NumberOfTargets = numberOfTargets;
            _fixedBuffer = 0;
        }
    }
}
