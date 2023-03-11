using NUnit.Framework;
using Titan.Windows;

namespace Titan.Tests;

public class WindowEventQueueTests
{
    [Test]
    public void HasEvents_NoEvents_ReturnFalse()
    {
        var queue = new WindowEventQueue();

        Assert.That(queue.HasEvents, Is.False);
    }

    [Test]
    public void HasEvents_SingleEvent_ReturnTrue()
    {
        var queue = new WindowEventQueue();

        queue.Push(new WindowEvent());

        Assert.That(queue.HasEvents, Is.True);
    }

    [Test]
    public void TryPeekEvent_NoEvents_ReturnFalse()
    {
        var queue = new WindowEventQueue();

        var result = queue.TryPeekEvent(out _);

        Assert.That(result, Is.False);
    }

    [Test]
    public void TryPeekEvent_SingleEvent_ReturnTrue()
    {
        var queue = new WindowEventQueue();
        queue.Push(new WindowEvent());

        var result = queue.TryPeekEvent(out _);

        Assert.That(result, Is.True);
    }

    [Test]
    public void TryPeekEvent_SingleEvent_SetEvent()
    {
        var queue = new WindowEventQueue();
        queue.Push(new WindowEvent { Value = 12 });

        queue.TryPeekEvent(out var result);

        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.As<WindowEvent>().Value, Is.EqualTo(12));
    }


    [Test]
    public void TryPeekEvent_MultipleEventMultipleCalls_SetSameEvent()
    {
        var queue = new WindowEventQueue();
        queue.Push(new WindowEvent { Value = 12 });
        queue.Push(new WindowEvent { Value = 13 });

        queue.TryPeekEvent(out _);
        queue.TryPeekEvent(out var result);

        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.As<WindowEvent>().Value, Is.EqualTo(12));
    }

    [Test]
    public void TryReadEvent_NoEvents_ReturnFalse()
    {
        var queue = new WindowEventQueue();

        var result = queue.TryReadEvent(out _);

        Assert.That(result, Is.False);
    }

    [Test]
    public void TryReadEvent_SingleEvent_ReturnTrue()
    {
        var queue = new WindowEventQueue();
        queue.Push(new WindowEvent());

        var result = queue.TryReadEvent(out _);

        Assert.That(result, Is.True);
    }

    [Test]
    public void TryReadEvent_SingleEventMultipleCalls_ReturnFalse()
    {
        var queue = new WindowEventQueue();
        queue.Push(new WindowEvent());

        queue.TryReadEvent(out _);
        var result = queue.TryReadEvent(out _);

        Assert.That(result, Is.False);
    }

    [Test]
    public void TryReadEvent_SingleEvent_SetEvent()
    {
        var queue = new WindowEventQueue();
        queue.Push(new WindowEvent { Value = 12 });

        queue.TryReadEvent(out var result);

        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.As<WindowEvent>().Value, Is.EqualTo(12));
    }

    [Test]
    public void TryReadEvent_MultipleEventsMultipleReads_SetEvent()
    {
        var queue = new WindowEventQueue();
        queue.Push(new WindowEvent { Value = 12 });
        queue.Push(new WindowEvent { Value = 13 });

        queue.TryReadEvent(out _);
        queue.TryReadEvent(out var result);

        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.As<WindowEvent>().Value, Is.EqualTo(13));
    }

    [Test]
    public void TryReadEvent_WithWrapAround_ReturnEvent()
    {
        var queue = new WindowEventQueue();

        for (var i = 0; i < WindowEventQueue.MaxEvents + 10; ++i)
        {
            queue.Push(new WindowEvent { Value = 12 + i });
            queue.TryReadEvent(out _);
        }

        queue.Push(new WindowEvent { Value = 3 });
        queue.TryReadEvent(out var result);

        Assert.That(result.Id, Is.EqualTo(2));
        Assert.That(result.As<WindowEvent>().Value, Is.EqualTo(3));
    }

    private struct WindowEvent : IWindowEvent
    {
        public static uint Id => 2;
        public int Value;
    }
}
