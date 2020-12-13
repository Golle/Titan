using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;

namespace Titan.Core.Messaging
{
    // TODO: replace this with the implementation in the ECS
    public class EventQueue : IEventQueue
    {
        private delegate void SwapDelegate();
        private readonly IList<SwapDelegate> _swaps = new List<SwapDelegate>();

        public void Initialize(IEventTypeProvider eventTypeProvider)
        {
            Debug.Assert(_swaps.Count == 0, "EventQueue has already been initialized");
            foreach (var type in eventTypeProvider.GetEventTypes())
            {
                var messageQueueType = typeof(EventQueueInternal<>).MakeGenericType(type);
                var method = messageQueueType.GetMethod("Swap", BindingFlags.NonPublic | BindingFlags.Static)?.CreateDelegate<SwapDelegate>();
                Debug.Assert(method != null, "Failed to find a static non public Swap method on the internal event queue.");
                _swaps.Add(method);
                LOGGER.Debug("{0} registered", type);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push<T>(in T @event) where T : struct => EventQueueInternal<T>.Push(@event);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> GetEvents<T>() where T : struct => EventQueueInternal<T>.GetEvents();

        public void Update()
        {
            foreach (var swap in _swaps)
            {
                swap();
            }
        }
    }
}
