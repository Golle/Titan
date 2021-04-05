using System;
using System.Collections.Concurrent;
using System.Threading;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Threading;
using static System.Int32;

namespace Titan.Assets
{


    

   

    public interface IAssetLoader<T>
    {
        T LoadFromData(ReadOnlySpan<byte> data);
    }

    public class AssetHandle<T>
    {


        internal void OnComplete()
        {
            LOGGER.Debug("REsources loaded");
        }
    }

    public interface IAssetManager
    {
        AssetHandle<T> Load<T>(string identifier);
    }

    internal class AssetManager : IAssetManager
    {
        private readonly FileSystem _fileSystem;
        private readonly AsyncFileLoader _fileLoader = new ();

        public AssetManager(FileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public AssetHandle<T> Load<T>(string identifier)
        {
            var path = _fileSystem.GetFullPath(identifier);
            var handle = new AssetHandle<T>();
            
            _fileLoader.Load(path, task =>
            {
                IOWorkerPool.QueueWorkerItem(item =>
                {
                    LOGGER.Debug("Loading resource");

                    handle.OnComplete();
                    task.Dispose();
                }, handle);
            });

            return handle;
        }

        public void Update()
        {

        }
    }
}
