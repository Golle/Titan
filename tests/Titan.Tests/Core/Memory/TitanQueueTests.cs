using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Titan.Core.Memory;

namespace Titan.Tests.Core.Memory;
[Ignore("Disabled for now since the queue is not completed.")]
internal unsafe class TitanQueueTests
{
    // use a single block of memory for all tests
    private void* _memory;

    [OneTimeSetUp]
    public void OneTimeSetup()
        => _memory = NativeMemory.Alloc(sizeof(long) * 10_000);

    [OneTimeTearDown]
    public void OnTimeTearDown()
        => NativeMemory.Free(_memory);


    //NOTE(Jens): reset memory at the start of each test to avoid any false data.
    [SetUp]
    public void Setup()
        => Unsafe.InitBlock(_memory, 0, sizeof(long) * 10_000);

    [Test]
    public void HasItems_NewQueue_ReturnFalse()
    {
        var queue = AllocQueue<ulong>(100);

        var result = queue.HasItems;

        Assert.That(result, Is.False);
    }

    [Test]
    public void HasItems_OneItem_ReturnTrue()
    {
        var queue = AllocQueue<ulong>(100);
        queue.Push(1);

        var result = queue.HasItems;

        Assert.That(result, Is.True);
    }

    [Test]
    public void Pop_SingleItem_ReturnItem()
    {
        var queue = AllocQueue<ulong>(100);
        queue.Push(1);

        var result = queue.Pop();

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void TryPop_NoItems_ReturnFalse()
    {
        var queue = AllocQueue<ulong>(100);

        var result = queue.TryPop(out _);

        Assert.That(result, Is.False);
    }

    [Test]
    public void TryPop_OneItem_ReturnTrue()
    {
        var queue = AllocQueue<ulong>(100);
        queue.Push(1);

        var result = queue.TryPop(out _);

        Assert.That(result, Is.True);
    }

    [Test]
    public void TryPop_OneItem_SetItem()
    {
        var queue = AllocQueue<ulong>(100);
        queue.Push(1);

        queue.TryPop(out var result);

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void TryPeek_NoItems_ReturnFalse()
    {
        var queue = AllocQueue<ulong>(100);

        var result = queue.TryPeek(out _);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Pop_MultipleItems_ReturnBoth()
    {
        var queue = AllocQueue<ulong>(100);
        queue.Push(1);
        queue.Push(2);

        var item1 = queue.Pop();
        var item2 = queue.Pop();

        Assert.That(item1, Is.EqualTo(1));
        Assert.That(item2, Is.EqualTo(2));
    }

    [Test]
    public void Pop_FullQueue_ReturnFirst()
    {
        var queue = AllocQueue<ulong>(3);
        queue.Push(1);
        queue.Push(2);
        queue.Push(3);

        var result = queue.Pop();

        Assert.That(result, Is.EqualTo(1));
    }


    private TitanQueue<T> AllocQueue<T>(uint count) where T : unmanaged
        => new((T*)_memory, count);
}

