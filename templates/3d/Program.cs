using Titan;
using Titan.Assets;
using Titan.Audio;
using Titan.Core.Logging;
using Titan.Core.Maths;
using Titan.Graphics;
using Titan.Input;
using Titan.Runners;
using Titan.Setup.Configs;

#if DEBUG || CONSOLE_LOGGING
using var _ = Logger.Start<ConsoleLogger>(1_000);

const bool UseRawAssets = true;

var devAssetFolder = Path.GetFullPath($"{AppContext.BaseDirectory}../../../../../assets");
var devPakFolder = Path.Combine(devAssetFolder, "bin");
var devEngineFolder = Path.GetFullPath(@"$TITAN_FOLDER_PATH$");
var devConfig = new AssetsDevConfiguration(devAssetFolder, devPakFolder, devEngineFolder, UseRawAssets);

#else
using var _ = Logger.Start(new FileLogger(Path.GetFullPath("logs", AppContext.BaseDirectory)), 1_000);
#endif

App.Create(new AppCreationArgs())
#if DEBUG
    .AddConfig(devConfig)

#endif
    .AddModule<GraphicsModule>()
    .AddModule<InputModule>()
    .AddModule<AudioModule>()
    .AddConfig(GraphicsConfig.Default with
    {
#if DEBUG
        Debug = true,
#else
        Debug = false,
#endif
        Vsync = true,
        AllowTearing = false,
        ClearColor = Color.Red,
    })
    .AddConfig(WindowConfig.Default with
    {
        Title = "$PROJECT_NAME$",
        Windowed = true,
        //AlwaysOnTop = true,
        Width = 1024,
        Height = 768,
        Resizable = true
    })
    .UseRunner<WindowedRunner>()
    .Build()
    .Run()
    ;
