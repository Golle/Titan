using Titan.GraphicsV2.Rendering;

namespace Titan.GraphicsV2.D3D11.Shaders
{
    internal readonly struct InputLayoutDescription
    {
        internal readonly string Name;
        internal readonly uint Slot;
        internal readonly TextureFormats Format;
        //internal D3D11_INPUT_CLASSIFICATION Classification; // TODO: add support for instance shaders

        public InputLayoutDescription(string name, TextureFormats format, uint slot = 0)
        {
            Name = name;
            Format = format;
            Slot = slot;
        }
    }
}
