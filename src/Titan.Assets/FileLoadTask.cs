using System;
using System.Runtime.InteropServices;

namespace Titan.Assets
{
    internal class FileLoadTask : IDisposable
    {
        private readonly Action<FileLoadTask> _callback;
        public string Filename { get; }
        public bool IsLoaded { get; private set; }

        private unsafe void* _data;
        private int _size;

        internal ReadOnlySpan<byte> Data
        {
            get
            {
                unsafe
                {
                    return new ReadOnlySpan<byte>(_data, _size);
                }
            }
        }

        internal FileLoadTask(string filename, Action<FileLoadTask> callback)
        {
            _callback = callback;
            Filename = filename;
        }

        internal unsafe void OnComplete(void* buffer, int size)
        {
            _data = buffer;
            _size = size;
            IsLoaded = true;
            _callback(this);
        }

        ~FileLoadTask() => Dispose();
        public unsafe void Dispose()
        {
            if (_data != null)
            {
                Marshal.FreeHGlobal((nint)_data);
                _data = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
