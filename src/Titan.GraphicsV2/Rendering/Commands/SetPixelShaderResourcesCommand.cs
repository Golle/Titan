using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Rendering.Commands
{
    [StructLayout(LayoutKind.Sequential, Size = sizeof(uint) * 2 + sizeof(long) * 6)]
    internal unsafe struct SetPixelShaderResourcesCommand
    {
        internal RenderCommandTypes Type;
        internal uint NumberOfViews;
        private long _fixedBuffer;
        internal ID3D11ShaderResourceView** Resources
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (ID3D11ShaderResourceView**) Unsafe.AsPointer(ref _fixedBuffer);
        }

        public SetPixelShaderResourcesCommand(uint numberOfViews)
        {
            Type = RenderCommandTypes.SetPixelShaderResource;
            NumberOfViews = numberOfViews;
            _fixedBuffer = 0;
        }
    }
}
