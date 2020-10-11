using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11
{
    public readonly struct InputLayoutDescriptor
    {
        public readonly string Name;
        public readonly DXGI_FORMAT Format;
        public InputLayoutDescriptor(string name, DXGI_FORMAT format)
        {
            Name = name;
            Format = format;
        }
    }
}
