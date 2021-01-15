using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Titan.Core.Threading
{
    public sealed class JobProgress : IDisposable
    {
        private uint _initialCount;

        private volatile uint _count;
        private readonly ManualResetEventSlim _event = new();
        private bool _disposed;
        public JobProgress(uint initialCount)
        {
            _count = _initialCount = initialCount;
            if (_initialCount == 0)
            {
                _event.Set();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void JobComplete()
        {
            ThrowIfDisposed();
            if (_count <= 0)
            {
                ThrowException();
            }
            
            var value = Interlocked.Decrement(ref _count);
            if (value == 0)
            {
                _event.Set();
            }
            [DoesNotReturn] static void ThrowException() => throw new InvalidOperationException("Count <= 0, can not decrement more.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => Reset(_initialCount);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset(uint count)
        {
            ThrowIfDisposed();
            _initialCount = _count = count;

            _event.Reset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Wait()
        {
            ThrowIfDisposed();
            _event.Wait();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsComplete() => _count == 0;

        ~JobProgress() => Dispose(false);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _event.Dispose();
                _disposed = true;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Conditional("DEBUG")] // Only check this when we're in debug mode.
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException($"{nameof(JobProgress)} has already been disposed.");
            }
        }
    }
}
