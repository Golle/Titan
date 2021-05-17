using Titan.Assets.Materials;

namespace Titan.Assets
{
    public static class Resources
    {
        public static MaterialManager Material { get; private set; }
        public static void Init()
        {
            Material = new MaterialManager();
        }

        public static void Terminate()
        {
            Material?.Dispose();
            Material = null;
        }

    }
}
