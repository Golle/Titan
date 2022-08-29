using System.Diagnostics.Metrics;
using System.Text;
using Titan.Assets.NewAssets;
using Titan.Core.Logging;
using Titan.Tools.Core.CommandLine;
using Titan.Tools.Core.Common;
using Titan.Tools.Core.Images;
using Titan.Tools.Core.Manifests;
using Titan.Tools.Packager;

Logger.Start<ConsoleLogger>();

//NOTE(Jens): this is only used in debug. hardcoded paths
//#if DEBUG
//const string manifestFile = @"F:\Git\Titan\samples\Titan.Sandbox\assets\sample_01.tmanifest";
//const string outputDir = @"F:\Git\Titan\samples\Titan.Sandbox\assets\bin\";
//const string generatedOutputDir = @"F:\Git\Titan\samples\Titan.Sandbox";
//const string @namespace = "Titan.Sandbox";

//args = $"package -m {manifestFile} -o {outputDir} -g {generatedOutputDir} -n {@namespace}".Split(' ');
//#endif

try
{
    Logger.Info("Welcome to the packager.");
    var result = await new CommandList<PipelineContext>($"{typeof(Program).Assembly.GetName().Name}.exe", "Titan - Packager")
        .WithCommand(new RootCommand<PipelineContext>("package")
            .WithOption(new Option<PipelineContext>("manifest")
            {
                Alias = "m",
                Description = "The absolute path to the manifest (Multiple manifests are allowed)",
                Callback = (context, path) => context with { ManifestPaths = context.ManifestPaths.Concat(new[] { path }).ToArray() },
                RequiresArguments = true,
                Validate = s => !string.IsNullOrWhiteSpace(s)
            })
            .WithOption(new Option<PipelineContext>("output")
            {
                Alias = "o",
                Description = "The output folder for the bundle",
                Validate = s => !string.IsNullOrWhiteSpace(s),
                RequiresArguments = true,
                Callback = (context, path) => context with { OutputPath = path }
            })
            .WithOption(new Option<PipelineContext>("generated")
            {
                Alias = "g",
                Description = "Output folder for generated C# files (Not implemented yet)",
                RequiresArguments = true,
                Validate = s => !string.IsNullOrWhiteSpace(s),
                Callback = (context, path) => context with { GeneratedCodePath = path }
            })
            .WithOption(new Option<PipelineContext>("namespace")
            {
                Alias = "n",
                Callback = (context, ns) => context with { Namespace = ns },
                RequiresArguments = true,
                Validate = s => !string.IsNullOrWhiteSpace(s),
                Description = "The namespace for the generated C# file."
            })
            .OnCommand(RunPipeline)
        )
        .Execute(args);
    if (result == null)
    {
        Logger.Error("Command returned null.");
        return -1;
    }

    if (result.Failed)
    {
        Logger.Error($"Command failed with reason: {result.Reason}");
        return -1;
    }

    Logger.Info("Command executed successfully");
}
catch (Exception e)
{
    Logger.Error($"Command failed with a {e.GetType().Name} with Message: {e.Message}");
    return -1;
}
finally
{
    Logger.Shutdown();
}

return 0;






static async Task<PipelineContext> RunPipeline(PipelineContext context)
{
    if (!ValidateContext(context))
    {
        return context with { Failed = true, Reason = "Validation failed." };
    }

    var outputPath = context.OutputPath!;
    //NOTE(Jens): this can be separated into different threads later (if needed). Just the C# code gen that should be single threaded.

    Logger.Info($"Manifests: {context.ManifestPaths.Length}");

    using ImageReader imageReader = new();

    for (var i = 0; i < context.ManifestPaths.Length; ++i)
    {
        var (assetRegistryFilename, titanPakFilename) = GenerateFileNames(i);

        var manifestPath = context.ManifestPaths[i]!;
        //NOTE(Jens): this will create different pak files for each manifest. is this what we want?
        using PackageStream packageExporter = new(1 * 1024 * 1024 * 1024); // 1GB 
        List<(string Name, AssetDescriptor Descriptor)> assetDescriptors = new();
        var manifest = await ReadManifest(manifestPath);
        if (manifest == null)
        {
            return context with { Failed = true, Reason = "Failed to get the manifest" };
        }
        Logger.Info($"Manifest: {manifest.Name}. Textures: {manifest.Textures.Count} Models: {manifest.Models.Count} Materials: {manifest.Materials.Count}");
        var basePath = Path.GetDirectoryName(manifestPath)!;
        // export images
        {
            foreach (var texture in manifest.Textures)
            {
                //NOTE(Jens): Load and convert images to DXGI_FORMAT
                var path = Path.Combine(basePath, texture.Path);
                var descriptor = ExportImage(path, imageReader, packageExporter);
                if (descriptor == null)
                {
                    return context with { Failed = true, Reason = $"Failed to export the image from path {path}" };
                }
                assetDescriptors.Add((texture.Name, descriptor.Value));
                Logger.Info($"Image {texture.Name}({texture.Path}) completed");
            }
        }
        // export models
        {
            foreach (var model in manifest.Models)
            {
                Logger.Warning($"Exporting model {model.Name}. (model exporting has not been implemented yet.)");
            }
        }

        // export materials
        {
            foreach (var material in manifest.Materials)
            {
                Logger.Warning($"Exporting material {material.Name}. (material exporting has not been implemented yet.)");
            }
        }
        var outputFile = Path.Combine(outputPath, titanPakFilename);
        {
            CreateIfNotExist(outputPath);
            Logger.Info($"Writing the Titan Package to file {outputFile}");
            await using var file = File.OpenWrite(outputFile);
            file.SetLength((long)packageExporter.Offset);
            file.Seek(0, SeekOrigin.Begin);
            packageExporter.Export(file);
            await file.FlushAsync();
            Logger.Info($"{packageExporter.Offset} bytes written");
        }

        if (string.IsNullOrWhiteSpace(context.GeneratedCodePath))
        {
            Logger.Warning("No generated code path specified, no index will be created.");
        }
        else
        {
            var fileContents = GenerateCSharpIndex(titanPakFilename, manifestPath, assetDescriptors, manifest.Name, context.Namespace);
            var generatedFilePath = Path.Combine(context.GeneratedCodePath, assetRegistryFilename);
            CreateIfNotExist(context.GeneratedCodePath);

            Logger.Info($"Writing the generated code to {generatedFilePath}");
            await using var writer = new StreamWriter(File.OpenWrite(generatedFilePath));
            writer.BaseStream.SetLength(0);
            await writer.WriteAsync(fileContents);
            await writer.FlushAsync();
            //await File.WriteAllTextAsync(generatedFilePath, fileContents);
        }
    }

    return context;



    static (string ManifestName, string PakFileName) GenerateFileNames(int index)
        => ($"AssetRegistry{++index,3:D3}.cs", $"data{index,3:D3}.titanpak");
}


static bool ValidateContext(PipelineContext context)
{
    var result = true;
    if (string.IsNullOrWhiteSpace(context.OutputPath))
    {
        Logger.Error($"{nameof(PipelineContext.OutputPath)} is null or empty.");
        result = false;
    }

    if (context.ManifestPaths.Length == 0)
    {
        Logger.Error($"{nameof(PipelineContext.ManifestPaths)} is empty.");
        result = false;
    }

    if (context.ManifestPaths.Any(string.IsNullOrWhiteSpace))
    {
        Logger.Error($"{nameof(PipelineContext.ManifestPaths)} has null or empty paths.");
        result = false;
    }
    return result;
}

static AssetDescriptor? ExportImage(string path, ImageReader imageReader, PackageStream packageStream)
{
    var image = imageReader.LoadImage(path);
    if (image == null)
    {
        Logger.Error($"Failed to read the image at path {path}");
        return null;
    }

    //NOTE(Jens): Add compressions (based on settings in manifest)

    // Read the position before writing the image (this is what will be used as a package reference)
    var offset = packageStream.Offset;
    packageStream.Write(image.Data);

    return new AssetDescriptor
    {
        Reference = { Offset = offset, Size = (ulong)image.Size },
        Type = AssetDescriptorType.Texture,
        Image = new()
        {
            Format = (uint)image.Format,
            Height = image.Height,
            Width = image.Width,
            Stride = image.Stride
        }
    };
}

static async Task<Manifest?> ReadManifest(string? path)
{
    if (string.IsNullOrWhiteSpace(path))
    {
        Logger.Error("Manifest path has not been set.");
        return null;
    }

    try
    {
        var bytes = await File.ReadAllBytesAsync(path);
        return Json.Deserialize<Manifest>(bytes);
    }
    catch (Exception e)
    {
        Logger.Error($"Read manifest from path {path} failed with {e.GetType().Name} and message {e.Message}");
        return null;
    }
}


static string GenerateCSharpIndex(string packageFile, string manifestPath, IReadOnlyList<(string Name, AssetDescriptor descriptor)> descriptors, string manifestName, string? @namespace)
{
    //NOTE(Jens): use a name that can be compiled in C#
    manifestName = ToPropertyName(manifestName);
    var unnamedCount = 0;
    var builder = new StringBuilder();
    builder.AppendLine($"// This is a generated file from {typeof(Program).Assembly.FullName}");
    if (!string.IsNullOrWhiteSpace(@namespace))
    {
        builder
            .AppendLine($"namespace {@namespace};")
            .AppendLine();
    }

    builder
        .AppendLine("public static partial class AssetRegistry")
        .AppendLine("{");

    builder.AppendLine($"\tpublic struct {manifestName} : {typeof(IManifestDescriptor).FullName}")
        .AppendLine("\t{");

    builder
        .AppendLine($"\t\tpublic static string ManifestFile => \"{Path.GetFileName(manifestPath)}\";")
        .AppendLine($"\t\tpublic static string TitanPackageFile => \"{Path.GetFileName(packageFile)}\";");

    builder
            .AppendLine("\t\tpublic static partial class Textures")
            .AppendLine("\t\t{");

    foreach (var (name, descriptor) in descriptors)
    {
        string propertyName;
        if (string.IsNullOrWhiteSpace(name))
        {
            Logger.Warning($"Unnamed {descriptor.Type}, will use a default name.");
            propertyName = $"UnnamedAsset{++unnamedCount,4:D4}";
        }
        else
        {
            propertyName = ToPropertyName(name);
        }

        builder.AppendLine($"\t\t\tpublic static readonly {typeof(AssetDescriptor).FullName} {propertyName} = {DescriptorToString(descriptor)}");
    }
    builder.AppendLine("\t\t}")
        .AppendLine("\t}")
        .AppendLine("}");


    builder.Replace("\t", new string(' ', 4));
    return builder.ToString();
    static string DescriptorToString(in AssetDescriptor descriptor) =>
        descriptor switch
        {
            { Type: AssetDescriptorType.Texture } => $"new() {{ Reference = {{ Offset = {descriptor.Reference.Offset}, Size = {descriptor.Reference.Size}}}, Type = {typeof(AssetDescriptorType).FullName}.{descriptor.Type}, Image = new() {{ Format = {descriptor.Image.Format}, Height = {descriptor.Image.Height}, Width = {descriptor.Image.Width}, Stride = {descriptor.Image.Stride} }} }};",
            _ => throw new NotImplementedException($"Type {descriptor.Type} has not been implemented yet.")
        };
}


static string ToPropertyName(string name)
{
    Span<char> buffer = stackalloc char[name.Length];
    var count = 0;
    if (!char.IsLetter(name[0]))
    {
        buffer[count++] = '_';
    }

    var makeUpper = true;
    foreach (var character in name)
    {
        //NOTE(Jens): add more illegal characters when we need it. 
        if (character is ' ' or '-' or '_')
        {
            makeUpper = true;
            continue;
        }
        buffer[count++] = makeUpper ? char.ToUpper(character) : character;
        makeUpper = false;
    }

    return new string(buffer[..count]);
}

static void CreateIfNotExist(string folder)
{
    if (!Directory.Exists(folder))
    {
        Directory.CreateDirectory(folder);
    }
}
