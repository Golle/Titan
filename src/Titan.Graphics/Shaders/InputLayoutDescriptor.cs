using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Shaders
{
    public record InputLayoutDescriptor(string Name, DXGI_FORMAT Format, uint Slot = 0, D3D11_INPUT_CLASSIFICATION Classification = D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_VERTEX_DATA);
}
