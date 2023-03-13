using Titan.Core;
using Titan.Events;

namespace Titan.Assets;

public record struct AssetLoadRequested(Handle<Asset> Handle) : IEvent;
public record struct AssetUnloadRequested(Handle<Asset> Handle) : IEvent;
public record struct AssetReloadRequested(Handle<Asset> Handle) : IEvent;
public record struct AssetReloadCompleted(Handle<Asset> Handle, AssetDescriptor Descriptor) : IEvent;
public record struct AssetLoadCompleted(Handle<Asset> Handle, AssetDescriptor Descriptor) : IEvent;
public record struct AssetUnloadCompleted(Handle<Asset> Handle, AssetDescriptor Descriptor) : IEvent;
