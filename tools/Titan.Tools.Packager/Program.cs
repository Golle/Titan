using Titan.Core.Logging;
using Titan.Tools.Core.CommandLine;


Logger.Start<ConsoleLogger>();

try
{
    Logger.Info("Welcome to the packager.");

    var result = await new CommandList<Context>("packager.exe", "Titan - Packager")
        .WithCommand(new RootCommand<PackageContext>("package")
            .WithOption(new Option<PackageContext>("manifest")
            {
                Alias = "m",
                Description = "The absolute path to the manifest",
                Callback = (context, path) => context with { ManifestPath = path },
                RequiresArguments = true,
                Validate = s => !string.IsNullOrWhiteSpace(s)
            })
            .WithOption(new Option<PackageContext>("output")
            {
                Alias = "o"
            })
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
finally
{
    Logger.Shutdown();
}

return 0;




public record Context
{
    public bool Failed { get; init; }

    public string? Reason { get; init; }
}

public record PackageContext : Context
{
    public string? ManifestPath { get; init; }
}


/*
 *CLI tool
 * *.exe package -m [path to manifest] -o [output path] -og [output path for gen C# file]
 * *.exe package -p [path to project(multiple manifests)] -o [output path] -og [output path for gen C# file] - nope
 *
 */
