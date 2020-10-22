using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Titan.Core.Messaging
{
    public class EventQueueTest
    {
        public void Test()
        {
            var queue = new EventQueue();

            queue.Push(new TestEvent(100));
            queue.Push(new TestEvent(120));
            queue.Push(new TestEvent(130));
            queue.Push(new TestEvent(150));
            queue.Push(new TestEvent(160));

            // should not print anything until after the swap
            Console.WriteLine($"Event Count: {queue.GetEvents<TestEvent>().Length} (Expected 0)");
            foreach (ref readonly var @event in queue.GetEvents<TestEvent>())
            {
                Console.WriteLine($@"Value: {@event.a}");
            }

            queue.Update();
            queue.Push(new TestEvent(200));
            // should print 5
            Console.WriteLine($"Event Count: {queue.GetEvents<TestEvent>().Length}  (Expected 5)");
            foreach (ref readonly var @event in queue.GetEvents<TestEvent>())
            {
                Console.WriteLine($@"Value: {@event.a}");
            }
            queue.Update();
            // should print 1
            Console.WriteLine($"Event Count: {queue.GetEvents<TestEvent>().Length}  (Expected 1)");
            foreach (ref readonly var @event in queue.GetEvents<TestEvent>())
            {
                Console.WriteLine($@"Value: {@event.a}");
            }
        }
    }

    [TitanEvent]
    readonly struct TestEvent
    {
        public readonly int a;

        public TestEvent(int a)
        {
            this.a = a;
        }
    }

    internal interface IEventQueue
    {
        void Push<T>(in T @event) where T : struct;
        ReadOnlySpan<T> GetEvents<T>() where T : struct;
    }

    internal class EventQueue : IEventQueue
    {
        private delegate void SwapDelegate();

        private readonly IList<SwapDelegate> _swaps = new List<SwapDelegate>();
        public EventQueue()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes().Where(t => t.GetCustomAttributes(false).Any(o => o.GetType() == typeof(TitanEventAttribute))))
                        .ToArray();

            foreach (var type in types)
            {
                var messageQueueType = typeof(EventQueueInternal<>).MakeGenericType(type);
                _swaps.Add(messageQueueType.GetMethod("Swap", BindingFlags.NonPublic | BindingFlags.Static).CreateDelegate<SwapDelegate>());
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

    [AttributeUsage(AttributeTargets.Struct)]
    internal class TitanEventAttribute : Attribute
    {
    }

    public class EventQueueInternal<T> where T : struct
    {
        private static readonly int _maxEvents;

        private static readonly T[] _messages;

        private static int _count;

        private static bool _high;
        
        private static int _start;
        private static int _length;

        static EventQueueInternal()
        {
            _maxEvents = 1000;
            _messages = new T[_maxEvents * 2];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Push(in T @event) => _messages[Interlocked.Increment(ref _count) - 1] = @event;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<T> GetEvents() => new ReadOnlySpan<T>(_messages, _start, _length);

        // ReSharper disable once UnusedMember.Local
        private static void Swap()
        {
            if (_high)
            {
                _start = _maxEvents;
                _length = _count - _maxEvents;
                _count = 0;
            }
            else
            {
                _start = 0;
                _length = _count;
                _count = _maxEvents;
            }
            _high = !_high;
        }
    }
}
