using System;
using System.Collections.Generic;
using System.IO;
using Titan.ECS.Systems;
using Titan.Graphics.Pipeline;

namespace Titan
{
    public record DisplayConfigurationFile(string Path);
    public record DisplayConfiguration(string Title, uint Width, uint Height, uint RefreshRate);
    public record PipelineConfigurationFile(string Path);
    public record LoggerConfiguration();
    public record AssetsDirectory(string Path);
    public record SystemsConfiguration(string Name, Type Type, string[] Dependencies);
    public record EventsConfiguration(uint MaxEventQueueSize);

    internal record GameConfiguration(AssetsDirectory AssetsDirectory, SystemsConfiguration[] Systems, DisplayConfiguration DisplayConfiguration, LoggerConfiguration LoggerConfiguration, PipelineConfiguration PipelineConfiguration, EventsConfiguration EventsConfiguration, Type Startup);

    public class GameConfigurationBuilder
    {
        private readonly List<SystemsConfiguration> _systems = new ();
        private AssetsDirectory _assetsDirectory;
        private DisplayConfiguration _displayConfiguration;
        private LoggerConfiguration _loggerConfiguration;
        
        private PipelineConfigurationFile _pipelineConfigurationFile;
        private DisplayConfigurationFile _displayConfigurationFile;
        private EventsConfiguration _eventConfiguration;
        private static readonly EventsConfiguration DefaultEventConfiguration = new(10_000);
        private Type _startupType;

        public GameConfigurationBuilder WithStartup<T>() where T : IStartup
        {
            _startupType = typeof(T);
            return this;
        }

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

        public GameConfigurationBuilder WithDisplayConfigurationFile(DisplayConfigurationFile configurationFile)
        {
            _displayConfigurationFile = configurationFile;
            return this;
        }
        public GameConfigurationBuilder WithDisplayConfiguration(DisplayConfiguration configuration)
        {
            _displayConfiguration = configuration;
            return this;
        }

        public GameConfigurationBuilder WithPipelineConfigurationFromFile(PipelineConfigurationFile configurationFile)
        {
            _pipelineConfigurationFile = configurationFile;
            return this;
        }

        public GameConfigurationBuilder WithEventsConfiguration(EventsConfiguration eventsConfiguration)
        {
            _eventConfiguration = eventsConfiguration;
            return this;
        }
        internal GameConfiguration Build(ConfigurationFileLoader loader)
        {
            Validate();

            string GetPath(string filename) => Path.Combine(_assetsDirectory.Path, filename);
            
            var pipelineConfiguration = loader.ReadConfig<PipelineConfiguration>(GetPath(_pipelineConfigurationFile.Path));
            var displayConfiguration = _displayConfiguration ?? loader.ReadConfig<DisplayConfiguration>(GetPath(_displayConfigurationFile.Path));
            var eventConfiguration = _eventConfiguration ?? DefaultEventConfiguration;
            
            return new (_assetsDirectory, _systems.ToArray(), displayConfiguration, _loggerConfiguration, pipelineConfiguration, eventConfiguration, _startupType);
        }

        private void Validate()
        {
            if(_assetsDirectory == null)
            {
                throw new InvalidOperationException($"{nameof(AssetsDirectory)} is not set. Use {nameof(WithAssetsDirectory)} to configure.");
            }
            if (_displayConfiguration == null && _displayConfigurationFile == null)
            {
                throw new InvalidOperationException($"{nameof(DisplayConfiguration)} is not set. Use {nameof(WithDisplayConfiguration)} or {nameof(WithDisplayConfigurationFile)} to configure.");
            }
            if (_loggerConfiguration == null)
            {
                throw new InvalidOperationException($"{nameof(LoggerConfiguration)} is not set. Use {nameof(WithDefaultConsoleLogger)} to configure.");
            }
            if (_pipelineConfigurationFile == null)
            {
                throw new InvalidOperationException($"{nameof(PipelineConfigurationFile)} is not set. Use {nameof(WithPipelineConfigurationFromFile)} to configure.");
            }
            if (_startupType == null)
            {
                throw new InvalidOperationException($"A Startup class has not been set. Use {nameof(WithStartup)} to configure.");
            }
        }
    }
}

