using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Titan.BundleBuilder;
using Titan.BundleBuilder.Pipeline;

const string InputPath = @"f:\git\titan\assets";

var modelConverterPipeline = new PipelineBuilder<ModelContext>()
    .Use(new WavefrontObjParser())
    .Use(new WavefrontObjConverter())
    .Use(new MaterialConverter())
    .Build();


var fileContents = File.ReadAllText(Path.Combine(InputPath, "bundle_01.json"));
var bundle = JsonSerializer.Deserialize<BundleDescriptor>(fileContents) ?? throw new InvalidOperationException("Failed to open bundle");


var modelTasks = bundle
    .Meshes
    .Select(async m => await modelConverterPipeline(new ModelContext {AssetsPath = InputPath, ModelDescriptor = m}));
var models = await Task.WhenAll(modelTasks);


Console.WriteLine("Hello World!");

internal record BundleContext
{
    internal ModelContext[] Models;
}



