using Titan.Assets;
using Titan.Core;
using Titan.Events;
using Titan.Systems;

namespace Titan.Editor;

internal struct AssetHotReloadSystem : ISystem
{
    private EventsWriter<AssetReloadRequested> _writer;
    private ObjectHandle<AssetChangeDetector> _changeDetector;

    public void Init(in SystemInitializer init)
    {
        _changeDetector = init.GetManagedApi<AssetChangeDetector>();
        _writer = init.GetEventsWriter<AssetReloadRequested>();
    }

    public void Update()
    {
        var detector = _changeDetector.Value;
        var count = detector.ChangeCount;
        Span<Handle<Asset>> buffer = stackalloc Handle<Asset>[count];
        detector.GetHandles(buffer);
        foreach (var handle in buffer)
        {
            if (handle.IsValid)
            {
                _writer.Send(new AssetReloadRequested(handle));
            }
        }
    }

    public bool ShouldRun() => _changeDetector.Value.ChangeCount > 0;
}
