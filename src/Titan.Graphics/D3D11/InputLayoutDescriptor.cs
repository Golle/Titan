using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11
{
    public readonly struct InputLayoutDescriptor
    {
        public readonly string Name;
        public readonly DXGI_FORMAT Format;
        public readonly D3D11_INPUT_CLASSIFICATION Classification;

        public InputLayoutDescriptor(string name, DXGI_FORMAT format, D3D11_INPUT_CLASSIFICATION classification = D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_VERTEX_DATA)
        {
            Classification = classification;
            Name = name;
            Format = format;
        }
    }
}
