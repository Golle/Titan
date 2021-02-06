using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal unsafe struct InputLayout
    {
        internal ID3D11InputLayout* Layout;
        internal InputLayout(ID3D11InputLayout* layout)
        {
            Layout = layout;
        }
    }
}
