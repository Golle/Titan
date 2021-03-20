using System;using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Titan.BundleBuilder;
using Titan.BundleBuilder.Bundles;
using Titan.BundleBuilder.Models;
using Titan.BundleBuilder.Models.Pipeline;
using Titan.BundleBuilder.Textures;

Configuration.Init(args);

var modelConverterPipeline = new PipelineBuilder<ModelContext>()
    .Use(new WavefrontObjParser())
    .Use(new WavefrontObjConverter())
    .Use(new MaterialConverter())
    .Build();

var textureConverterPipeline = new PipelineBuilder<TextureContext>()
    .Use(new LoadAndConvertTexture())
    .Build();


var bundlePipeline = new PipelineBuilder<BundleContext>()
    .Use(new WriteBundle())
    .Use(new CreateDataChunks())
    .Build();

var fileContents = File.ReadAllText(Path.Combine(Configuration.AssetsPath, "bundle_01.json"));
var bundle = JsonSerializer.Deserialize<BundleSpecification>(fileContents) ?? throw new InvalidOperationException("Failed to open bundle");

var modelTasks = bundle
    .Models
    .Select(async m => await modelConverterPipeline(new ModelContext(m)));

var models = await Task.WhenAll(modelTasks);


var textureSpecifications = bundle
    .Textures
    .Select(t => t with {Filename = Path.GetFullPath(Path.Combine(Configuration.AssetsPath, t.Filename))})
    .Concat(
        models
            .SelectMany(m => m.Materials.SelectMany(mat => new[] {mat.DiffuseMapPath, mat.NormalMapPath}))
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Select(path => new TextureSpecification(Path.GetFileNameWithoutExtension(path), path))
    )
    .Distinct()
    .ToArray();


//TODO: We could skip this step if we add an additional identifier for texture names (or ignore the texture names for textures loaded by through materials)
if (textureSpecifications.GroupBy(t => t.Name).Any(g => g.Count() > 1))
{
    throw new DuplicateNameException($"Multiple textures with the same name.");
}

var textureTasks = textureSpecifications
    .Select(async t => await textureConverterPipeline(new TextureContext(t)))
    .ToArray();
var textures = await Task.WhenAll(textureTasks);

var result = await bundlePipeline(new BundleContext(bundle.Name)
{
    Textures = textures,
    Models = models
});

Console.WriteLine("Hello World!");


