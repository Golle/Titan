using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Rendering.Commands
{
    [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 2 + sizeof(long) * 6)]
    internal unsafe struct SetVertexShaderResourcesCommand
    {
        internal RenderCommandTypes Type;
        internal uint NumberOfViews;
        private long _fixedBuffer;
        internal ID3D11ShaderResourceView** Resources
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (ID3D11ShaderResourceView**)Unsafe.AsPointer(ref _fixedBuffer);
        }
        public SetVertexShaderResourcesCommand(uint numberOfViews)
        {
            Type = RenderCommandTypes.SetVertexShaderResource;
            NumberOfViews = numberOfViews;
            _fixedBuffer = 0;
        }
    }
}
