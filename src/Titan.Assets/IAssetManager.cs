using System;
using System.Collections.Concurrent;
using System.Threading;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Threading;
using static System.Int32;

namespace Titan.Assets
{


    public abstract class IOThreadWorkerItem
    {
        internal abstract void Execute();
    }

    internal sealed class IOThreadWorkerItem<T> : IOThreadWorkerItem
    {
        private readonly Action<T> _callback;
        private readonly T _state;
        public IOThreadWorkerItem(Action<T> callback, in T state)
        {
            _callback = callback;
            _state = state;
        }

        internal override void Execute()
        {
            _callback(_state);
        }
    }

    
    public static class IOWorkerPool
    {
        private static Thread[] _threads;
        private static bool _active;
        private static readonly Semaphore _semaphore = new(0, MaxValue);
        private static readonly ConcurrentQueue<IOThreadWorkerItem> _workerItems = new();
        
        public static void Initialize(int threadCount = 4, int maxQueuedJobs = 1000)
        {
            if (_threads != null)
            {
                throw new InvalidOperationException($"{nameof(IOWorkerPool)} has already been initialized.");
            }

            _threads = new Thread[threadCount];
            _active = true;
            for (var i = 0; i < threadCount; i++)
            {
                var thread = _threads[i] = new Thread(Run);
                thread.Name = $"IOThread #{i}";
                thread.Priority = ThreadPriority.BelowNormal;
                thread.IsBackground = true;
                thread.Start(i);
            }
        }

        public static void Shutdown()
        {
            if (_active)
            {
                _active = false;
                _semaphore.Release(_threads.Length);
                foreach (var thread in _threads)
                {
                    thread.Join();
                }
                _threads = null;
            }
        }

        private static void Run(object obj)
        {
            var threadId = (int) obj;
            while (_active)
            {
                _semaphore.WaitOne();
                if (_workerItems.TryDequeue(out var workerItem))
                {
                    workerItem.Execute();
                }
            }
        }

        public static void QueueWorkerItem<T>(Action<T> action, in T state)
        {
            _workerItems.Enqueue(new IOThreadWorkerItem<T>(action, state));
            _semaphore.Release();
        }
    }

   

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
