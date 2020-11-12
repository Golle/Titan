using System.IO;

namespace Titan.AssetConverter.Files
{
    internal interface IByteWriter
    {
        void Write<T>(Stream stream, in T[] data) where T : unmanaged;
        void Write<T>(Stream stream, in T data) where T : unmanaged;
    }
}
