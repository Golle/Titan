namespace Titan.Graphics.Pipeline.Configuration
{
    public interface IPipelineConfigurationLoader
    {
        PipelineConfiguration Load(string filename);
    }
}
