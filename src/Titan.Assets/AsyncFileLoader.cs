using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Titan.Assets
{
    internal class AsyncFileLoader : IDisposable
    {
        private Thread _thread;
        private bool _active = true;
        private readonly SemaphoreSlim _semaphore = new(0);
        private readonly ConcurrentQueue<FileLoadTask> _queue = new();

        public AsyncFileLoader()
        {
            _thread = new Thread(Start);
            _thread.Start();
        }

        private unsafe void Start()
        {
            while (_active)
            {
                _semaphore.Wait();
                if (!_queue.TryDequeue(out var task))
                {
                    continue;
                }

                var fileStream = File.Open(task.Filename, FileMode.Open, FileAccess.Read, FileShare.None);
                void* buffer = null;
                try
                {
                    var length = (int)fileStream.Length;
                    buffer = (void*)Marshal.AllocHGlobal(length);
                    fileStream.Read(new Span<byte>(buffer, length));
                    task.OnComplete(buffer, length);
                }
                catch
                {
                    //free the buffer in case of an exception
                    if (buffer != null)
                    {
                        Marshal.FreeHGlobal((nint)buffer);
                    }
                }
                finally
                {
                    fileStream.Dispose();
                }
            }
        }

        public FileLoadTask Load(string identifier, Action<FileLoadTask> callback)
        {
            var fileLoadTask = new FileLoadTask(identifier, callback);
            _queue.Enqueue(fileLoadTask);
            _semaphore.Release();
            return fileLoadTask;
        }

        public void Dispose()
        {
            _active = false;
            _semaphore.Release();
            _thread.Join();
            _thread = null;
        }
    }
}
