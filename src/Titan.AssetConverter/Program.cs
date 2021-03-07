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


var pipeline = new PipelineBuilder<MeshContext>()
    .Use(new MeshExporterMiddlware())
    .Use(new WavefronObjectReaderMiddleware())
    .Use(new ObjConverterMiddleware())
    .Use(new ModelBuilderMiddleware())
    .Build();


var tasks = Directory
    .GetFiles(input, "door.obj", SearchOption.AllDirectories)
    .Select(f => Task.Run(() => pipeline(context with {Filename = f})));

await Task.WhenAll(tasks);
