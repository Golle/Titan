## Titan
This is a C# (.NET 8) only Game Engine (Reboot #56)   
Titan currently uses DirectX 12 for graphics and XAudio2 for audio.   

Keep in mind that the development of this engine is in a very early stage and will have a lot of breaking changes.   

The engine is built with cross platform in mind but all parts have not been implemented.  
Missing features:
* Wayland/X11 window implementation
* Vulkan rendering API
* Audio implementation



## Builds
[![Titan](https://github.com/Golle/Titan/actions/workflows/titan.yml/badge.svg)](https://github.com/Golle/Titan/actions/workflows/titan.yml)

## Setup
There's a script that you can run that will setup the parts you need to start using the engine.

```powershell
./setup.ps1
```

## Sample projects
* [Titan in Space](https://github.com/Golle/TitanInSpace) a space invaders clone.

## How to

#### Solution and project file
1. Create a new C# console project in a path that's in the same directory as the titan root folder.
    ```
    /git/Titan
    /git/YourAmazingGame
    ```

2. Add the Titan engine projects to the solution.
See [Space.sln](https://github.com/Golle/TitanInSpace/blob/main/Space.sln) for an example.

3. Run manifest builder and open the built.tmanifest and run Cook Assets from the menu. This will build the engine shaders and put them in the titanpak file.
4. Create an assets folder in your project, open the manifest tool and select "Open or Create" and select that folder. It will create a .tmanifest  and a .tconfig file in that folder.
5. Add textures, audio and whatever you need for your game into the manifest, run "Cook assets" and fill out the Namespace, target directory etc for the generated files.   
Sample from the Space Invaders project
    ```json
    {
        "namespace" : "Space.Assets",
        "outputPath" : "bin",
        "generatedPath" : "..\\src\\Space\\Assets",
        "manifestStartId" : null
    }
    ```
6. Setup the entry point
    ```csharp
    using Space;
    using Titan;
    using Titan.Assets;
    using Titan.Audio;
    using Titan.Core.Logging;
    using Titan.Core.Maths;
    using Titan.Graphics;
    using Titan.Input;
    using Titan.Runners;
    using Titan.Setup.Configs;

    #if DEBUG
    using var _ = Logger.Start<ConsoleLogger>(10_000);

    const bool UseRawAssets = true;

    var devAssetFolder = Path.GetFullPath($"{AppContext.BaseDirectory}../../../../../assets");
    var devPakFolder = Path.Combine(devAssetFolder, "bin");
    var devEngineFolder = Path.GetFullPath($"{AppContext.BaseDirectory}../../../../../../Titan/");
    var devConfig = new AssetsDevConfiguration(devAssetFolder, devPakFolder, devEngineFolder, UseRawAssets);
    #else
    using var _ = Logger.Start(new FileLogger(Path.GetFullPath(AppContext.BaseDirectory, "MyAmazingGame", "logs", )), 10_000);
    #endif

    App.Create(new AppCreationArgs())
    #if DEBUG
        .AddConfig(devConfig)
    #endif
        .AddModule<GraphicsModule>()
        .AddModule<InputModule>()
        .AddModule<AudioModule>()
        .AddModule<GameModule>()
        .AddConfig(GraphicsConfig.Default with
        {
    #if DEBUG
            Debug = true,
    #else
            Debug = false,
    #endif
            Vsync = true,
            AllowTearing = false,
            ClearColor = Color.Red
        })
        .AddConfig(WindowConfig.Default with
        {
            Title = "This is my game",
            Windowed = true,
            //AlwaysOnTop = true, // Enable this if you want the window to always be on top
            Width = 1024,
            Height = 768,
            Resizable = true
        })
        .UseRunner<WindowedRunner>()
        .Build()
        .Run()
        ;

    ```

7. Start the game with F5!
8. See [GameModule.cs](https://github.com/Golle/TitanInSpace/blob/main/src/Space/GameModule.cs) in the TitanInSpace sample for a IModule implementation where it adds components, systems and assets.

## Tools
The tools that are used by titan to build the manifests.

#### Manifest Builder

Path: `tools/Titan.Tools.ManifestBuilder/Titan.Tools.ManifestBuilder.exe`

#### Packager

Path: `tools/Titan.Tools.Packager/Titan.Tools.Packager.exe`


## Roadmap
.... not yet decided, come back soon!