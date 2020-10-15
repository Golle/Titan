using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11
{
    public interface IResource
    {
        unsafe ID3D11Resource* AsResourcePointer();
        DXGI_FORMAT Format { get; }
    }
}
