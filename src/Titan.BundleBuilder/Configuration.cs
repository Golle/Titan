using System.IO;

namespace Titan.BundleBuilder
{
    internal static class Configuration
    {
        internal static string AssetsPath { get; private set; }
        internal static string ModelsPath { get; private set; }
        internal static string TexturesPath { get; private set; }
        public static string OutputPath { get; set; }

        internal static void Init(string[] args)
        {
            AssetsPath = @"f:\git\titan\assets";
            OutputPath = @"f:\git\titan\resources\bundles";
            TexturesPath = Path.Combine(AssetsPath, "textures");
            ModelsPath = Path.Combine(AssetsPath, "models");
        }
    }
}
