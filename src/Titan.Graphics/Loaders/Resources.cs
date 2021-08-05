using Titan.Graphics.Loaders.Materials;
using Titan.Graphics.Loaders.Models;

namespace Titan.Graphics.Loaders
{
    public static class Resources
    {
        public static MaterialManager Material { get; private set; }
        public static ModelManager Models { get; private set; }
        public static void Init()
        {
            Material = new MaterialManager();
            Models = new ModelManager();
        }

        public static void Terminate()
        {
            Material?.Dispose();
            Material = null;
            Models?.Dispose();
            Models = null;
        }

    }
}
