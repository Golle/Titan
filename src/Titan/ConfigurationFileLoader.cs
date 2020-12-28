using Titan.Core.Common;

namespace Titan
{
    internal class ConfigurationFileLoader
    {
        private readonly IFileReader _fileReader;
        private readonly IJsonSerializer _jsonSerializer;

        public ConfigurationFileLoader(IFileReader fileReader, IJsonSerializer jsonSerializer)
        {
            _fileReader = fileReader;
            _jsonSerializer = jsonSerializer;
        }
        
        public T ReadConfig<T>(string path) => _jsonSerializer.Deserialize<T>(_fileReader.ReadText(path));
    }
}
