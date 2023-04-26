// This is a generated file from Titan.Tools.Packager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
namespace Titan.Graphics;

internal static partial class AssetRegistry
{
    public readonly struct BuiltIn : Titan.Assets.IManifestDescriptor
    {
        public static uint Id => 0;
        public static string ManifestFile => "builtin.tmanifest";
        public static string TitanPackageFile => "builtin.titanpak";
        public static uint AssetCount => 10;
        public static Titan.Assets.AssetDescriptor[] AssetDescriptors { get; } =
        {
            new() { Id = 0, ManifestId = 0, Reference = { Offset = 0, Size = 65536}, Type = Titan.Assets.AssetDescriptorType.Texture, Image = new() { Format = 87, Height = 128, Width = 128, Stride = 512 } },
            new() { Id = 1, ManifestId = 0, Reference = { Offset = 65536, Size = 65536}, Type = Titan.Assets.AssetDescriptorType.Texture, Image = new() { Format = 87, Height = 128, Width = 128, Stride = 512 } },
            new() { Id = 2, ManifestId = 0, Reference = { Offset = 131072, Size = 1524}, Type = Titan.Assets.AssetDescriptorType.Shader, Shader = new() { ShaderType = Titan.Assets.ShaderType.Pixel } },
            new() { Id = 3, ManifestId = 0, Reference = { Offset = 132596, Size = 1824}, Type = Titan.Assets.AssetDescriptorType.Shader, Shader = new() { ShaderType = Titan.Assets.ShaderType.Vertex } },
            new() { Id = 4, ManifestId = 0, Reference = { Offset = 134420, Size = 3324}, Type = Titan.Assets.AssetDescriptorType.Shader, Shader = new() { ShaderType = Titan.Assets.ShaderType.Vertex } },
            new() { Id = 5, ManifestId = 0, Reference = { Offset = 137744, Size = 2880}, Type = Titan.Assets.AssetDescriptorType.Shader, Shader = new() { ShaderType = Titan.Assets.ShaderType.Pixel } },
            new() { Id = 6, ManifestId = 0, Reference = { Offset = 140624, Size = 3388}, Type = Titan.Assets.AssetDescriptorType.Shader, Shader = new() { ShaderType = Titan.Assets.ShaderType.Vertex } },
            new() { Id = 7, ManifestId = 0, Reference = { Offset = 144012, Size = 2880}, Type = Titan.Assets.AssetDescriptorType.Shader, Shader = new() { ShaderType = Titan.Assets.ShaderType.Pixel } },
            new() { Id = 8, ManifestId = 0, Reference = { Offset = 146892, Size = 1952}, Type = Titan.Assets.AssetDescriptorType.Shader, Shader = new() { ShaderType = Titan.Assets.ShaderType.Vertex } },
            new() { Id = 9, ManifestId = 0, Reference = { Offset = 148844, Size = 2416}, Type = Titan.Assets.AssetDescriptorType.Shader, Shader = new() { ShaderType = Titan.Assets.ShaderType.Pixel } },
        };
#if DEBUG
        public static object[] RawAssets { get; } =
        {
            new Titan.Tools.Core.Manifests.TextureItem{ Name = "splash", Path = @"textures\splash.png", Type = Titan.Tools.Core.Manifests.TextureType.PNG },
            new Titan.Tools.Core.Manifests.TextureItem{ Name = "ui", Path = @"textures\ui.png", Type = Titan.Tools.Core.Manifests.TextureType.PNG },
            new Titan.Tools.Core.Manifests.ShaderItem{ Name = "simple_ps_01", Path = @"shaders\simple_ps_01.hlsl", EntryPoint = "main", ShaderModel = Titan.Tools.Core.Shaders.ShaderModels.PS_6_5 },
            new Titan.Tools.Core.Manifests.ShaderItem{ Name = "simple_vs_01", Path = @"shaders\simple_vs_01.hlsl", EntryPoint = "main", ShaderModel = Titan.Tools.Core.Shaders.ShaderModels.VS_6_5 },
            new Titan.Tools.Core.Manifests.ShaderItem{ Name = "SpriteVS", Path = @"shaders\sprites.hlsl", EntryPoint = "VSMain", ShaderModel = Titan.Tools.Core.Shaders.ShaderModels.VS_6_5 },
            new Titan.Tools.Core.Manifests.ShaderItem{ Name = "SpritePS", Path = @"shaders\sprites.hlsl", EntryPoint = "PSMain", ShaderModel = Titan.Tools.Core.Shaders.ShaderModels.PS_6_5 },
            new Titan.Tools.Core.Manifests.ShaderItem{ Name = "Sprite2VS", Path = @"shaders\spritesv2.hlsl", EntryPoint = "VSMain", ShaderModel = Titan.Tools.Core.Shaders.ShaderModels.VS_6_5 },
            new Titan.Tools.Core.Manifests.ShaderItem{ Name = "Sprite2PS", Path = @"shaders\spritesv2.hlsl", EntryPoint = "PSMain", ShaderModel = Titan.Tools.Core.Shaders.ShaderModels.PS_6_5 },
            new Titan.Tools.Core.Manifests.ShaderItem{ Name = "FullscreenVS", Path = @"shaders\fullscreen.hlsl", EntryPoint = "VSMain", ShaderModel = Titan.Tools.Core.Shaders.ShaderModels.VS_6_5 },
            new Titan.Tools.Core.Manifests.ShaderItem{ Name = "FullscreenPS", Path = @"shaders\fullscreen.hlsl", EntryPoint = "PSMain", ShaderModel = Titan.Tools.Core.Shaders.ShaderModels.PS_6_5 },
        };
#else
        public static object[] RawAssets { get; } = System.Array.Empty<object>();
#endif
        public static class Textures
        {
            public static ref readonly Titan.Assets.AssetDescriptor Splash => ref AssetDescriptors[0];
            public static ref readonly Titan.Assets.AssetDescriptor Ui => ref AssetDescriptors[1];
            public static ref readonly Titan.Assets.AssetDescriptor SimplePs01 => ref AssetDescriptors[2];
            public static ref readonly Titan.Assets.AssetDescriptor SimpleVs01 => ref AssetDescriptors[3];
            public static ref readonly Titan.Assets.AssetDescriptor SpriteVS => ref AssetDescriptors[4];
            public static ref readonly Titan.Assets.AssetDescriptor SpritePS => ref AssetDescriptors[5];
            public static ref readonly Titan.Assets.AssetDescriptor Sprite2VS => ref AssetDescriptors[6];
            public static ref readonly Titan.Assets.AssetDescriptor Sprite2PS => ref AssetDescriptors[7];
            public static ref readonly Titan.Assets.AssetDescriptor FullscreenVS => ref AssetDescriptors[8];
            public static ref readonly Titan.Assets.AssetDescriptor FullscreenPS => ref AssetDescriptors[9];
        }
    }
}
