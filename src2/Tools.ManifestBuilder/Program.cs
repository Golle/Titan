using System;
using System.IO;
using System.Linq;
using Titan.Core.Logging;


Logger.Start();

const string assetsPath = @"F:\Git\Titan\assets";
const string outputPath = @"F:\Git\Titan\assetsV3";

Directory.CreateDirectory(outputPath);

Logger.Info($"Reading assets from {assetsPath}");

var a = Directory.EnumerateFiles(assetsPath, "*", SearchOption.AllDirectories)
    .Select(file => new {Path = file, Name = Path.GetFileNameWithoutExtension(file), Extension = Path.GetExtension(file) })
    .Select(asset =>
    {
        switch (asset.Extension.ToLowerInvariant())
        {
            case ".mtl":
                Logger.Trace("Ignoring material (Will be referenced in Obj files)");
                break;
            case ".png" or ".jpg": 
                Logger.Trace("Texture found");
                break;
            case ".obj":
                Logger.Trace("wavefront obj found");
                break;
                default:
                    Logger.Warning($"unknown extension {asset.Extension} for file {asset.Name}");
                    break;
        }

        return 1;
    })
    .ToArray();
    


Logger.Info<InvalidProgramException>("This is amazing");



Logger.Shutdown();
