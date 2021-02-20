using Titan.Core.Common;
using Titan.Core.IO;

namespace Titan.GraphicsV2.Rendering
{
    internal class RenderPipelineReader
    {
        private readonly IFileReader _fileReader;
        private readonly IJsonSerializer _jsonSerializer;

        public RenderPipelineReader(IFileReader fileReader, IJsonSerializer jsonSerializer)
        {
            _fileReader = fileReader;
            _jsonSerializer = jsonSerializer;
        }

        internal RenderPipelineSpecification[] Read(string identifier)
        {
            var value = _fileReader.ReadText(identifier);
            return _jsonSerializer.Deserialize<RenderPipelineSpecification[]>(value);
        }
    }
}
