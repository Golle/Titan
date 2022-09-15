using System.Text;
using System.Xml.Serialization;
using Titan.Core.Logging;
using Tools.AtlasBuilder;

Logger.Start();

var texturePath = @"F:\Art\UI\UIpack\Spritesheet";
var destinationPath = @"F:\Git\Titan\samples\Titan.Sandbox\assets\textures";

foreach (var file in Directory.EnumerateFiles(texturePath, "*.xml"))
{
    var name = Path.GetFileNameWithoutExtension(file).ToLower();
    
    Logger.Info($"Parsing file {name}");

    var folder = Path.GetDirectoryName(file) ?? throw new Exception("Failed to get the folder name.");
    var imageFilename = $"{name}.png";
    var image = Path.Combine(folder, imageFilename);


    if (!File.Exists(image))
    {
        Logger.Error($"Image file {image} not found.");
        continue;
    }

    using var fileStream = File.OpenRead(file);
    var result = (TextureAtlas?)new XmlSerializer(typeof(TextureAtlas)).Deserialize(fileStream);

    if (result == null)
    {
        Logger.Error($"Failed to parse file {file}");
        continue;
    }


    if (result.SubTextures == null)
    {
        Logger.Error($"No SubTextures found in {file}");
        continue;
    }

    var atlasName = $"{name}.atlas";
    var atlasDestinationPath = Path.Combine(destinationPath, atlasName);
    Logger.Info($"Write atlas to {atlasDestinationPath}");

    if (File.Exists(atlasDestinationPath))
    {
        Logger.Warning($"Atlas {name} already exists. Press enter to overwrite or space to skip.");
        ConsoleKey key;
        do
        {
            key = Console.ReadKey(true).Key;
        } while (key != ConsoleKey.Spacebar && key != ConsoleKey.Enter);

        if (key == ConsoleKey.Enter)
        {
            Logger.Warning($"Skipping {name}");
            continue;
        }
        Logger.Warning($"Overwrite {name}");
    }
    using var outputFile = File.OpenWrite(atlasDestinationPath);

    Write(outputFile, $"{result.SubTextures.Length};0\n");
    foreach (var subTexture in result.SubTextures)
    {
        Write(outputFile, $"{subTexture.X};{subTexture.Y};{subTexture.Width};{subTexture.Height};{subTexture.Name}\n");
    }

    var imageDestinationPath = Path.Combine(destinationPath, imageFilename);
    Logger.Info($"Write image to {imageDestinationPath}");
    File.Copy(image, imageDestinationPath, true);
    Logger.Info($"Completed file {name}");
}

Logger.Shutdown();

static void Write(Stream stream, ReadOnlySpan<char> text)
{
    Span<byte> buffer = stackalloc byte[512];
    var length = Encoding.UTF8.GetBytes(text, buffer);
    stream.Write(buffer[..length]);
}

namespace Tools.AtlasBuilder
{
    public class TextureAtlas
    {
        [XmlAttribute(AttributeName = "imagePath")]
        public string? ImagePath { get; set; }
        [XmlElement(ElementName = "SubTexture")]
        public SubTexture[]? SubTextures { get; set; }
    }

    public class SubTexture
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }
        [XmlAttribute("x")]
        public int X { get; set; }
        [XmlAttribute("y")]
        public int Y { get; set; }
        [XmlAttribute("width")]
        public int Width { get; set; }
        [XmlAttribute("height")]
        public int Height { get; set; }
    }
}
