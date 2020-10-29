namespace Titan
{
    public class EngineConfiguration
    {
        public string ResourceBasePath { get; init; }
        public uint Height { get; init; }
        public uint Width { get; init; }
        public string Title { get; init; }
        public uint RefreshRate { get; init; }
        public bool Debug { get; init; }
    }
}
