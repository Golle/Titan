using System.Runtime.CompilerServices;
using System.Threading;

namespace Titan.ECS.Components
{
    // TODO: might be better to use this when we need a string. Might cause memory leaks since we can't control the lifetime of the strings.
    // Maybe we can use a Handle<string> instead of TitanString, and all strings must be created as part of a component. Not sure how to do this though. 
    // Keep the ManagedComponent for now.
    internal static class TitanStringCache
    {
        private static readonly string[] Strings = new string[100_000];
        private static long _nextId;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Create(string value)
        {
            var id = Interlocked.Increment(ref _nextId);;
            Strings[id] = value;
            return id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Get(long id) => Strings[id];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Release(long id) => Strings[id] = null;
    }


    internal struct TitanString
    {
        private long Id;
        public TitanString(string value) => Id = TitanStringCache.Create(value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => TitanStringCache.Get(Id);
    }
}
