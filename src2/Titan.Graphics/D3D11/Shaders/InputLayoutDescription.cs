namespace Titan.Graphics.D3D11.Shaders
{
    public readonly struct InputLayoutDescription
    {
        public readonly string Name;
        public readonly uint Slot;

        public readonly TextureFormats Format;
        //internal D3D11_INPUT_CLASSIFICATION Classification; // TODO: add support for instance shaders

        public InputLayoutDescription(string name, TextureFormats format, uint slot = 0)
        {
            Name = name;
            Format = format;
            Slot = slot;
        }
    }
}
