using System.Diagnostics;
using Titan.Core.Logging;
using Titan.Graphics.Loaders.Fonts;
using Tools.FontBuilder.BitmapFonts;

Logger.Start();

//if (args.Length != 2)
//{
//    Logger.Error("Oh crap, you didn't add enough arguments to create the fonts.");
//    Logger.Shutdown();
//    return -1;
//}


static string GetPath(string path) => Path.IsPathRooted(path) ? path : Path.Combine(Directory.GetCurrentDirectory(), path);

var resourcePath = @"F:\Git\Titan\samples\Titan.Sandbox\resources\fonts"; //GetPath(args[0]);
var destinationPath = @"F:\Git\Titan\samples\Titan.Sandbox\assets\fonts"; //GetPath(args[1]);
var texturePath = @"F:\Git\Titan\samples\Titan.Sandbox\assets\textures"; //GetPath(args[2]);


// Make sure the directory exists
if (!Directory.Exists(destinationPath))
{
    Directory.CreateDirectory(destinationPath);
}
if (!Directory.Exists(texturePath))
{
    Directory.CreateDirectory(texturePath);
}


foreach (var fontFile in Directory.EnumerateFiles(resourcePath, "*.fnt"))
{
    var timer = Stopwatch.StartNew();
    Logger.Info($"Parsing file: {fontFile}");
    var name = Path.GetFileNameWithoutExtension(fontFile).Replace(' ', '_').ToLowerInvariant();
    Logger.Info($"Name: {name}");
    var bitmapFont = BitmapFontParser.ReadFromFile(fontFile);

    Logger.Info($"Font size: {bitmapFont.Info.Size}");
    Logger.Info($"Characters: {bitmapFont.Chars.Length}");
    Logger.Info($"Kernings: {bitmapFont.Kernings.Length}");
    Logger.Info($"Bitmap size: {bitmapFont.Bitmap.Length}");
    var kernings = bitmapFont.Kernings.Select(k => new KerningDescriptor
    {
        Amount = (short)k.Amount,
        First = k.First,
        Second = k.Second
    }).ToArray();

    var characters = bitmapFont.Chars.Select(c => new GlyphDescriptor
    {
        X = (short)c.X,
        Y = (short)c.Y,
        Width = (short)c.Width,
        Height = (short)c.Height,
        XOffset = (short)c.XOffset,
        YOffset = (short)c.YOffset,
        XAdvance = (short)c.XAdvance,
        Id = (char)c.Id
    }).ToArray();

    await File.WriteAllBytesAsync(Path.Combine(texturePath, $"{name}.png"), bitmapFont.Bitmap);
    await using var outputFile = File.Open(Path.Combine(destinationPath, $"{name}.fnt"), FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
    outputFile.SetLength(0);

    unsafe
    {
        var font = new FontDescriptor
        {
            FontSize = (ushort)bitmapFont.Info.Size,
            Base = (ushort)bitmapFont.Common.Base,
            LineHeight = (ushort)bitmapFont.Common.LineHeight,
            CharactersCount = (ushort)bitmapFont.Chars.Length,
            KerningsCount = (ushort)bitmapFont.Kernings.Length
        };
        outputFile.Write(new ReadOnlySpan<byte>(&font, sizeof(FontDescriptor)));
        
        fixed (GlyphDescriptor* pCharacters = characters)
        {
            outputFile.Write(new ReadOnlySpan<byte>(pCharacters, sizeof(GlyphDescriptor) * characters.Length));
        }

        fixed (KerningDescriptor* pKernings = kernings)
        {
            outputFile.Write(new ReadOnlySpan<byte>(pKernings, sizeof(KerningDescriptor) * kernings.Length));
        }
    }
    Logger.Info($"Export completed in {timer.Elapsed.TotalMilliseconds} ms");
}

Logger.Shutdown();
return 0;

