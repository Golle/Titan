using System;
using Titan.Core.Common;

namespace Titan.Graphics.Pipeline.Configuration
{
    internal class PipelineConfigurationLoader : IPipelineConfigurationLoader
    {
        private readonly IFileReader _fileReader;
        private readonly IJsonSerializer _jsonSerializer;

        public PipelineConfigurationLoader(IFileReader fileReader, IJsonSerializer jsonSerializer)
        {
            _fileReader = fileReader;
            _jsonSerializer = jsonSerializer;
        }

        public PipelineConfiguration Load(string filename)
        {
            var json = _fileReader.ReadText(filename);
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new InvalidOperationException($"The configuration file '{filename}' is either null or empty.");
            }
            return _jsonSerializer.Deserialize<PipelineConfiguration>(json);
        }
    }
}
