using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;using Titan.AssetConverter;


var converter = new ModelConverter();

var input = @"F:\Git\GameDev\resources\";
var output = @"F:\Git\Titan\resources\models\";

var files = Directory.GetFiles(input, "*.obj", SearchOption.AllDirectories);

string OutputPath(string filePath, string extension) => Path.ChangeExtension(Path.Combine(output, Path.GetFileName(filePath)), extension);

var tasks = files.Select(f => converter.Convert(f, OutputPath(f, ".dat"), OutputPath(f, ".mat"))).ToArray();
await Task.WhenAll(tasks);

Console.WriteLine("Hello world!");
