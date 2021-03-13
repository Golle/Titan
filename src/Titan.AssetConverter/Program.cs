using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Titan.AssetConverter.Pipeline;
using Titan.AssetConverter.Pipeline.Middlewares;


const string input = @"F:\Git\GameDev\resources\";

var context = new MeshContext
{
    OutputFolder = @"F:\Git\Titan\resources\models1\"
};

if (!Directory.Exists(context.OutputFolder))
{
    Logger.Info($"Creating output directory {context.OutputFolder}");
    Directory.CreateDirectory(context.OutputFolder);
}


var pipeline = new PipelineBuilder<MeshContext>()
    .Use(new MaterialExporterMiddleware())
    .Use(new MeshExporterMiddleware())
    .Use(new WavefronObjectReaderMiddleware())
    .Use(new ObjConverterMiddleware())
    .Use(new MaterialConverterMiddleware())
    .Build();


var tasks = Directory
    .GetFiles(input, "*.obj", SearchOption.AllDirectories)
    .Select(f => Task.Run(() => pipeline(context with {Filename = f})));

await Task.WhenAll(tasks);
