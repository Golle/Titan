using Titan.Assets;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;

namespace Titan.Audio.Playback;

internal class AudioCommandQueue
{
    private TitanArray<AudioCommand> _commands;

    private TitanArray<AudioCommand> _current;
    private TitanArray<AudioCommand> _previous;

    private volatile int _count;
    private int _maxCount;
    private AudioRegistry _registry;
    private IMemoryManager _memoryManager;

    public int Count => _count;
    public bool Init(IMemoryManager memoryManager, AudioRegistry registry, AudioConfig config)
    {
        if (!memoryManager.TryAllocArray(out _commands, config.MaxQueuedAudioCommands * 2, true))
        {
            Logger.Error<AudioCommandQueue>("Failed to allocate memory for the command queue.");
            return false;
        }

        _maxCount = (int)config.MaxQueuedAudioCommands;

        _current = _commands[.._maxCount];
        _previous = _commands.Slice(_maxCount, _maxCount);
        _registry = registry;
        _memoryManager = memoryManager;
        return true;
    }

    public void Enqueue(in AudioCommand command)
    {
        var index = Interlocked.Increment(ref _count) - 1;
        _current[index] = command;
    }

    public Handle<Audio> CreateAndEnqueue(in Handle<Asset> asset, in PlaybackSettings settings)
    {
        var handle = _registry.Create(asset, settings);
        if (handle.IsInvalid)
        {
            Logger.Error<AudioCommandQueue>("Failed to create the AudioHandle. Audio creating will be ignored.");
            return 0;
        }
        Enqueue(new AudioCommand { Command = AudioCommands.Play, Audio = handle, });
        return handle;
    }

    public ReadOnlySpan<AudioCommand> PopCommandsAndSwap()
    {
        var commands = _count == 0
            ? ReadOnlySpan<AudioCommand>.Empty
            : _current[.._count].AsReadOnlySpan();
        Swap();
        return commands;
    }

    private void Swap()
    {
        (_current, _previous) = (_previous, _current);
        _count = 0;
    }


    public void Shutdown()
    {
        _memoryManager?.Free(ref _commands);
        _previous = _current = default;
        _memoryManager = null;
    }
}

