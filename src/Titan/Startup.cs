using System;
using System.Collections.Generic;
using System.IO;
using Titan.ECS.Systems;
using Titan.IOC;

namespace Titan
{
    public class GameConfigurationBuilder
    {

        private readonly List<SystemsConfiguration> _systems = new ();
        private AssetsDirectory _assetsDirectory;
        private DisplayConfiguration _displayConfiguration;


        public GameConfigurationBuilder WithSystem<T>(string name, params string[] dependencies) where T : IEntitySystem
        {
            _systems.Add(new(name, typeof(T), dependencies ?? Array.Empty<string>()));
            return this;
        }

        public GameConfigurationBuilder WithAssetsDirectory(AssetsDirectory assetsDirectory)
        {
            _assetsDirectory = assetsDirectory;
            return this;
        }

        public GameConfigurationBuilder WithDisplayConfiguration(DisplayConfiguration configuration)
        {
            _displayConfiguration = configuration;
            return this;
        }

        

        internal GameConfiguration Build()
        {
            Validate();
            return new GameConfiguration(_assetsDirectory, _systems.ToArray());
        }

        private void Validate()
        {
            if(_assetsDirectory == null)
            {
                throw new InvalidOperationException($"{nameof(AssetsDirectory)} is not set. Use {nameof(WithAssetsDirectory)} to configure.");
            }
            if (_displayConfiguration == null)
            {
                throw new InvalidOperationException($"{nameof(DisplayConfiguration)} is not set. Use {nameof(WithDisplayConfiguration)} to configure.");
            }
        }
    }

    public record DisplayConfiguration(string Title, uint Width, uint Height, uint RefreshRate)
    {
        public static DisplayConfiguration FromFile(string file) => System.Text.Json.JsonSerializer.Deserialize<DisplayConfiguration>(File.ReadAllText(file));
    }

    public record GameConfiguration(AssetsDirectory AssetsDirectory, SystemsConfiguration[] Systems);
    public record AssetsDirectory(string Path);
    public record SystemsConfiguration(string Name, Type Type, string[] Dependencies);


    public class Application : IDisposable
    {
        private Application(GameConfiguration configuration)
        {

        }

        public static Application Start(GameConfiguration configuration)
        {

            return new(configuration);
        }

        public void Dispose()
        {
        }
    }

    public class Startup
    {
        public Startup()
        {
            
        }

        public void Start()
        {

        }

        private void Configure(IContainer container)
        {
            //var device = container
        }
    }
}
