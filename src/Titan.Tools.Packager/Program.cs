using System.Text;
using Titan.Assets;
using Titan.Core.Logging;
using Titan.Tools.Core.Audio;
using Titan.Tools.Core.CommandLine;
using Titan.Tools.Core.Common;
using Titan.Tools.Core.Images;
using Titan.Tools.Core.Manifests;
using Titan.Tools.Core.Shaders;
using Titan.Tools.Packager;
using Titan.Tools.Packager.PreReqs;

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
                Description = "The path to the manifest (Multiple manifests are allowed)",
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
            .WithOption(new Option<PipelineContext>("libpath")
            {
                Alias = "l",
                Description = "The path to the external libraries used for packaging, for example DXC(DirectX Shader Compiler)",
                RequiresArguments = true,
                Validate = s => !string.IsNullOrWhiteSpace(s),
                Callback = (context, path) => context with { LibraryPath = path }
            })
            .WithOption(new Option<PipelineContext>("id")
            {
                Description = "The ManifestID to start with. 0 is reserved for Built in Engine Assets.",
                RequiresArguments = true,
                Callback = (context, value) => context with { ManifestStartId = int.Parse(value!) },
                Validate = value => int.TryParse(value, out var _)
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

    if (context.LibraryPath == null)
    {
        Logger.Info("No library path specified, will download DXC.");
        var result = await DownloadDXCCompiler.Download();
        if (result.Failed)
        {
            return context with { Failed = true, Reason = result.Error };
        }
        context = context with { LibraryPath = result.Data };
    }

    if (context.LibraryPath != null)
    {
        Logger.Info($"Using library path {context.LibraryPath} for shader compilation.");
        ShaderCompiler.SetShaderCompilerDllFolder(context.LibraryPath);
    }

    var outputPath = context.OutputPath!;
    //NOTE(Jens): this can be separated into different threads later (if needed). 

    Logger.Info($"Manifests: {context.ManifestPaths.Length}");

    using ImageReader imageReader = new();

    for (var i = 0; i < context.ManifestPaths.Length; ++i)
    {
        var manifestPath = context.ManifestPaths[i]!;
        var (assetRegistryFilename, titanPakFilename) = GenerateFileNames(i, manifestPath);
        //NOTE(Jens): this will create different pak files for each manifest. is this what we want?
        using PackageStream packageExporter = new(1 * 1024 * 1024 * 1024); // 1GB 
        List<(ManifestItem Item, AssetDescriptor Descriptor)> assetDescriptors = new();
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
                assetDescriptors.Add((texture, descriptor.Value));
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
        // export shaders
        {
            foreach (var shader in manifest.Shaders)
            {
                var path = Path.Combine(basePath, shader.Path);
                var descriptor = CompileShader(path, shader, packageExporter);
                if (descriptor == null)
                {
                    return context with { Failed = true, Reason = $"Failed to compile the shader from path {path}" };
                }
                assetDescriptors.Add((shader, descriptor.Value));
                Logger.Info($"Shader {shader.Name} completed");
            }
        }

        // export audio

        {
            foreach (var sound in manifest.Audio)
            {
                var path = Path.Combine(basePath, sound.Path);

                var descriptor = ExportAudio(path, sound, packageExporter);
                if (descriptor == null)
                {
                    return context with { Failed = true, Reason = $"Failed to export the sound clip from path {path}" };
                }
                assetDescriptors.Add((sound, descriptor.Value));
                Logger.Info($"Sound clip {sound.Name} completed");

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
            var manifestId = (uint)(i + context.ManifestStartId);
            var fileContents = GenerateCSharpIndex(manifestId, titanPakFilename, manifestPath, assetDescriptors, manifest.Name, context.Namespace);
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



    static (string ManifestName, string PakFileName) GenerateFileNames(int index, string manifestPath)
        => ($"AssetRegistry{++index,3:D3}.cs", $"{Path.GetFileNameWithoutExtension(manifestPath).ToLowerInvariant()}.titanpak");
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

static AssetDescriptor? ExportAudio(string path, AudioItem audioItem, PackageStream packageStream)
{
    var result = WaveReader.Read(path);
    if (!result.Success)
    {
        Logger.Error($"Failed to read the wave file with error: {result.Error}");
        return null;
    }
    var offset = packageStream.Offset;
    var clip = result.Sound!;

    packageStream.Write(clip.Data);

    return new AssetDescriptor
    {
        Reference = { Offset = offset, Size = (ulong)clip.Data.Length },
        Type = AssetDescriptorType.Audio,
        Audio = new AudioAssetDescriptor
        {
            Channels = clip.Format.nChannels,
            SamplesPerSecond = clip.Format.nSamplesPerSec,
            BitsPerSample = clip.Format.wBitsPerSample
        }
    };
}

static AssetDescriptor? CompileShader(string path, ShaderItem shader, PackageStream packageStream)
{
    var result = ShaderCompiler.Compile(path, shader.EntryPoint, shader.ShaderModel);
    if (!result.Succeeded)
    {
        Logger.Error($"Failed to compile the shader at path {path}, with error message: {result.Error}");
        return null;
    }
    var offset = packageStream.Offset;
    var byteCode = result.GetByteCode();
    packageStream.Write(byteCode);
    return new AssetDescriptor
    {
        Reference = { Offset = offset, Size = (ulong)byteCode.Length },
        Type = AssetDescriptorType.Shader,
        Shader = new ShaderAssetDescriptor
        {
            ShaderType = shader.ShaderModel switch
            {
                >= ShaderModels.CS_5_0 and <= ShaderModels.CS_5_1 => ShaderType.Compute,
                >= ShaderModels.PS_5_0 and <= ShaderModels.PS_5_1 => ShaderType.Pixel,
                >= ShaderModels.VS_5_0 and <= ShaderModels.VS_5_1 => ShaderType.Vertex,

                >= ShaderModels.CS_6_0 and <= ShaderModels.CS_6_7 => ShaderType.Compute,
                >= ShaderModels.PS_6_0 and <= ShaderModels.PS_6_7 => ShaderType.Pixel,
                >= ShaderModels.VS_6_0 and <= ShaderModels.VS_6_7 => ShaderType.Vertex,
                _ => throw new NotSupportedException($"Shader model {shader.ShaderModel} can't be converted.")
            }
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


static string GenerateCSharpIndex(uint manifestId, string packageFile, string manifestPath, IReadOnlyList<(ManifestItem Item, AssetDescriptor Descriptor)> descriptors, string manifestName, string? @namespace)
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
        .AppendLine("internal static partial class AssetRegistry")
        .AppendLine("{");

    builder.AppendLine($"\tpublic readonly struct {manifestName} : {typeof(IManifestDescriptor).FullName}")
        .AppendLine("\t{");

    //NOTE(Jens): We should add a "hash/checksum" of the file, if we want to validate that it's the same one. Could affect performance though.
    builder
        .AppendLine($"\t\tpublic static uint {nameof(IManifestDescriptor.Id)} => {manifestId};")
        .AppendLine($"\t\tpublic static string {nameof(IManifestDescriptor.ManifestFile)} => \"{Path.GetFileName(manifestPath)}\";")
        .AppendLine($"\t\tpublic static string {nameof(IManifestDescriptor.TitanPackageFile)} => \"{Path.GetFileName(packageFile)}\";")
        .AppendLine($"\t\tpublic static uint {nameof(IManifestDescriptor.AssetCount)} => {descriptors.Count};");

    {
        //NOTE(Jens): add the managed array of AssetDescriptors
        builder.AppendLine($"\t\tpublic static {typeof(AssetDescriptor).FullName}[] {nameof(IManifestDescriptor.AssetDescriptors)} {{ get; }} =")
            .AppendLine("\t\t{");
        for (var i = 0; i < descriptors.Count; ++i)
        {
            builder.AppendLine($"\t\t\t{DescriptorToString(i, manifestId, descriptors[i].Descriptor)},");
        }
        builder.AppendLine("\t\t};");
    }

    {
        builder.AppendLine("#if DEBUG");
        //NOTE(Jens): Add the manifest item for all assets with a if debug flag

        builder.AppendLine($"\t\tpublic static object[] {nameof(IManifestDescriptor.RawAssets)} {{ get; }} =")
            .AppendLine("\t\t{");
        foreach (var descriptor in descriptors)
        {
            builder.AppendLine($"\t\t\t{ItemToString(descriptor)},");
        }
        builder.AppendLine("\t\t};");

        builder.AppendLine("#else");
        builder.AppendLine($"\t\tpublic static object[] {nameof(IManifestDescriptor.RawAssets)} {{ get; }} = {typeof(Array).FullName}.Empty<object>();");
        builder.AppendLine("#endif");
    }


    {
        //NOTE(Jens): add the Textures (and other assets?)
        //NOTE(Jens): This needs to be refactored and split up into components now, wont be nice with the different types of assets.
        builder
            .AppendLine("\t\tpublic static class Textures")
            .AppendLine("\t\t{");

        //foreach (var (name, descriptor) in descriptors)
        for (var i = 0; i < descriptors.Count; ++i)
        {
            var (item, descriptor) = descriptors[i];
            string propertyName;
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                Logger.Warning($"Unnamed {descriptor.Type}, will use a default name.");
                propertyName = $"UnnamedAsset{++unnamedCount,4:D4}";
            }
            else
            {
                propertyName = ToPropertyName(item.Name);
            }
            builder.AppendLine($"\t\t\tpublic static ref readonly {typeof(AssetDescriptor).FullName} {propertyName} => ref {nameof(IManifestDescriptor.AssetDescriptors)}[{i}];");
        }
        builder.AppendLine("\t\t}");
    }

    builder
        .AppendLine("\t}")
        .AppendLine("}");


    builder.Replace("\t", new string(' ', 4));
    return builder.ToString();
    static string DescriptorToString(int id, uint manifestId, in AssetDescriptor descriptor) =>
        descriptor switch
        {
            { Type: AssetDescriptorType.Texture } => $"new() {{ Id = {id}, ManifestId = {manifestId}, Reference = {{ Offset = {descriptor.Reference.Offset}, Size = {descriptor.Reference.Size}}}, Type = {typeof(AssetDescriptorType).FullName}.{descriptor.Type}, Image = new() {{ Format = {descriptor.Image.Format}, Height = {descriptor.Image.Height}, Width = {descriptor.Image.Width}, Stride = {descriptor.Image.Stride} }} }}",
            { Type: AssetDescriptorType.Shader } => $"new() {{ Id = {id}, ManifestId = {manifestId}, Reference = {{ Offset = {descriptor.Reference.Offset}, Size = {descriptor.Reference.Size}}}, Type = {typeof(AssetDescriptorType).FullName}.{descriptor.Type}, Shader = new() {{ ShaderType = {typeof(ShaderType).FullName}.{descriptor.Shader.ShaderType} }} }}",
            { Type: AssetDescriptorType.Audio } => $"new() {{ Id = {id}, ManifestId = {manifestId}, Reference = {{ Offset = {descriptor.Reference.Offset}, Size = {descriptor.Reference.Size}}}, Type = {typeof(AssetDescriptorType).FullName}.{descriptor.Type}, Audio = new() {{ Channels = {descriptor.Audio.Channels}, BitsPerSample = {descriptor.Audio.BitsPerSample}, SamplesPerSecond = {descriptor.Audio.SamplesPerSecond} }} }}",
            _ => throw new NotImplementedException($"Type {descriptor.Type} has not been implemented yet.")
        };

    static string ItemToString(in (ManifestItem Item, AssetDescriptor Descriptor) value) =>
        value.Item switch
        {
            ShaderItem shader => $"new {typeof(ShaderItem).FullName}{{ {nameof(ManifestItem.Name)} = \"{shader.Name}\", {nameof(ShaderItem.Path)} = @\"{shader.Path}\", {nameof(ShaderItem.EntryPoint)} = \"{shader.EntryPoint}\", {nameof(ShaderItem.ShaderModel)} = {typeof(ShaderModels).FullName}.{shader.ShaderModel} }}",
            AudioItem audio => $"new {typeof(AudioItem).FullName}{{ {nameof(ManifestItem.Name)} = \"{audio.Name}\", {nameof(AudioItem.Path)} = @\"{audio.Path}\" }}",
            MaterialItem material => $"new {typeof(MaterialItem).FullName}{{ {nameof(ManifestItem.Name)} = \"{material.Name}\" }}",
            ModelItem model => $"new {typeof(ModelItem).FullName}{{ {nameof(ManifestItem.Name)} = \"{model.Name}\", {nameof(ModelItem.Path)} = @\"{model.Path}\" }}",
            TextureItem texture => $"new {typeof(TextureItem).FullName}{{ {nameof(ManifestItem.Name)} = \"{texture.Name}\", {nameof(TextureItem.Path)} = @\"{texture.Path}\", {nameof(TextureItem.Type)} = {typeof(TextureType).FullName}.{texture.Type} }}",
            _ => throw new NotImplementedException($"Type {value.Item.GetType().Name} has not been implemented yet.")
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
