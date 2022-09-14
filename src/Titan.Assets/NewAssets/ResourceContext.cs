using Titan.Core;

namespace Titan.Assets.NewAssets;
/// <summary>
/// The ResourceContext is used for the creation of the resources. It has an optional "state" and Load/Unload functions
/// </summary>
internal unsafe struct ResourceContext
{
    private void* _context;
    private delegate*<void*, ReadOnlySpan<byte>, Handle> _create;
    private delegate*<void*, in Handle, void> _destroy;
    public Handle Create(ReadOnlySpan<byte> data) => _create(_context, data);
    public void Destroy(in Handle handle) => _destroy(_context, handle);
    public bool IsInitialized() => _create != null && _destroy != null;
    public static ResourceContext Create<TResourceType, TCreatorType>(void* context)
        where TResourceType : unmanaged
        where TCreatorType : unmanaged, IResourceCreator<TResourceType> =>
        new()
        {
            _context = context,
            _create = &FunctionWrapper<TResourceType, TCreatorType>.Create,
            _destroy = &FunctionWrapper<TResourceType, TCreatorType>.Destroy
        };

    private struct FunctionWrapper<TResourceType, TCreatorType>
        where TResourceType : unmanaged
        where TCreatorType : unmanaged, IResourceCreator<TResourceType>
    {
        public static Handle Create(void* context, ReadOnlySpan<byte> buffer) => TCreatorType.Create(context, buffer).Value;
        public static void Destroy(void* context, in Handle handle) => TCreatorType.Destroy(context, handle);
    }
}
