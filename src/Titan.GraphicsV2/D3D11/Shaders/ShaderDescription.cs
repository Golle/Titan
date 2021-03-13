namespace Titan.GraphicsV2.D3D11.Shaders
{
    internal struct ShaderDescription
    {
        internal string Source;
        internal string Version;
        internal string Entrypoint;

        public ShaderDescription(string source, string entrypoint, string version)
        {
            Source = source;
            Entrypoint = entrypoint;
            Version = version;
        }
    }
}
