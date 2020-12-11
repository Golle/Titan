using System;
using System.Collections.Generic;
using System.IO;
using Titan.ECS.Systems;

namespace Titan
{

    public record DisplayConfiguration(string Title, uint Width, uint Height, uint RefreshRate)
    {
        public static DisplayConfiguration FromFile(string file) => System.Text.Json.JsonSerializer.Deserialize<DisplayConfiguration>(File.ReadAllText(file));
    }

    public record PipelineConfiguration(string Path);

    public record LoggerConfiguration();
    public record AssetsDirectory(string Path);
    public record SystemsConfiguration(string Name, Type Type, string[] Dependencies);

    internal record GameConfiguration(AssetsDirectory AssetsDirectory, SystemsConfiguration[] Systems, DisplayConfiguration DisplayConfiguration, LoggerConfiguration LoggerConfiguration, PipelineConfiguration PipelineConfiguration);

    public class GameConfigurationBuilder
    {
        private readonly List<SystemsConfiguration> _systems = new ();
        private AssetsDirectory _assetsDirectory;
        private DisplayConfiguration _displayConfiguration;
        private LoggerConfiguration _loggerConfiguration;
        private PipelineConfiguration _pipelineConfiguration;

        public GameConfigurationBuilder WithDefaultConsoleLogger()
        {
            _loggerConfiguration = new LoggerConfiguration();
            return this;
        }

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

        public GameConfigurationBuilder WithPipelineConfiguration(PipelineConfiguration configuration)
        {
            _pipelineConfiguration = configuration;
            return this;
        }

        internal GameConfiguration Build()
        {
            Validate();
            return new (_assetsDirectory, _systems.ToArray(), _displayConfiguration, _loggerConfiguration, _pipelineConfiguration);
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
            if (_loggerConfiguration == null)
            {
                throw new InvalidOperationException($"{nameof(LoggerConfiguration)} is not set. Use {nameof(WithDefaultConsoleLogger)} to configure.");
            }
            if (_pipelineConfiguration == null)
            {
                throw new InvalidOperationException($"{nameof(PipelineConfiguration)} is not set. Use {nameof(WithPipelineConfiguration)} to configure.");
            }
        }
    }
}

