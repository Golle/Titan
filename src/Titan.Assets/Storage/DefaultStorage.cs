using System;
using System.Diagnostics;
using Titan.Core;
using Titan.Core.Logging;

namespace Titan.Assets.Storage
{
    public class DefaultStorage<T> : IAssetStorage<T> where T : class
    {
        private readonly T[] _items;
        private static readonly int Offset = new Random().Next(100, 100000);
        public DefaultStorage(uint count)
        {
            _items = new T[count];
        }

        public T Pop(in Handle<T> handle)
        {
            Logger.Debug<DefaultStorage<T>>("Pop asset");
            var index = handle.Value - Offset;
            Debug.Assert(index >= 0 && index < _items.Length, "The handle is out of bounds");

            var item = _items[index];
            _items[index] = null;
            return item;
        }

        public Handle<T> Push(in T asset)
        {
            Logger.Debug<DefaultStorage<T>>("Push asset");
            for (var i = 0; i < _items.Length; ++i)
            {
                if (_items[i] == null)
                {
                    _items[i] = asset;
                    return i + Offset;
                }
            }
            Debug.Assert(false, $"Storage is full, the maximum of {_items.Length} has been reached.");
            return 0;
        }
    }
}
