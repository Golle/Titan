using System.IO;

namespace Titan.BundleBuilder
{
    internal static class Configuration
    {
        internal static string AssetsPath { get; private set; }
        internal static string ModelsPath { get; private set; }
        internal static string TexturesPath { get; private set; }
        internal static void Init(string[] args)
        {
            AssetsPath = @"f:\git\titan\assets";
            TexturesPath = Path.Combine(AssetsPath, "textures");
            ModelsPath = Path.Combine(AssetsPath, "models");
        }
    }
}
