using Titan.Core;
using Titan.Core.Memory;
using Titan.Core.Threading;

namespace Titan.Assets
{
    internal struct AssetState
    {
        public string Identifier;
        public LoadState State;

        public Handle<WorkerPool> FileHandle;
        public Handle<WorkerPool> MetadataFileHandle;

        public MemoryChunk<byte> FileBytes;
        
        public IAsset Asset;
    }
}
