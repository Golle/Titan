using System;

namespace Titan.Core.Threading
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
}
