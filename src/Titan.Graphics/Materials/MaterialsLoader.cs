using Titan.Core.Common;
using Titan.Core.IO;

namespace Titan.Graphics.Materials
{
    internal class MaterialsLoader : IMaterialsLoader
    {
        private readonly IFileReader _fileReader;
        private readonly IJsonSerializer _jsonSerializer;

        public MaterialsLoader(IFileReader fileReader, IJsonSerializer jsonSerializer)
        {
            _fileReader = fileReader;
            _jsonSerializer = jsonSerializer;
        }
        public MaterialConfiguration[] LoadMaterials(string filename)
        {
            var json = _fileReader.ReadText(filename);
            var materials = _jsonSerializer.Deserialize<MaterialConfiguration[]>(json);

            return materials;
        }
    }
}
