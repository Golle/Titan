using Titan.Core;
using Titan.Core.Memory;

namespace Titan.Assets.Creators;

/// <summary>
/// The ResourceContext is used for the creation of the resources. It has a context and Create/Destroy functions
/// </summary>
internal readonly unsafe struct ResourceContext
{
    private readonly void* _context;
    private readonly delegate*<void*, void> _release;
    private readonly delegate*<void*, in AssetDescriptor, TitanBuffer, Handle> _create;
    private readonly delegate*<void*, in Handle, in AssetDescriptor, TitanBuffer, bool> _recreate;
    private readonly delegate*<void*, Handle, void> _destroy;
    public bool IsValid() => _context != null;
    public void* AsPointer() => _context;
    public Handle Create(in AssetDescriptor descriptor, TitanBuffer data) => _create(_context, descriptor, data);
    public bool Recreate(in Handle handle, in AssetDescriptor descriptor, TitanBuffer data) => _recreate(_context, handle, descriptor, data);
    public void Destroy(Handle handle) => _destroy(_context, handle);
    public void Release() => _release(_context);
    public ResourceContext(void* context, in ResourceCreatorDescriptor descriptor)
    {
        _context = context;
        _create = descriptor.Create;
        _release = descriptor.Release;
        _destroy = descriptor.Destroy;
        _recreate = descriptor.Recreate;
    }
}
