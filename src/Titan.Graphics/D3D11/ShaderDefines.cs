namespace Titan.Graphics.D3D11
{
    public readonly struct ShaderDefines
    {
        public readonly string Name;
        public readonly string Value;
        public ShaderDefines(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
