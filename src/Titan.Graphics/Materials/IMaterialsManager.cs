namespace Titan.Graphics.Materials
{
    

    public interface IMaterialsManager
    {
        public MaterialHandle CreateFromConfiguration(in MaterialConfiguration materialConfiguration);
        ref readonly Material this[in MaterialHandle handle] { get; }
    }
}
