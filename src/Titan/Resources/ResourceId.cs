using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;

namespace Titan.Resources;

[DebuggerDisplay("ResourceId: {_id}")]
internal readonly struct ResourceId
{
    private readonly uint _id;
    public ResourceId(uint id) => _id = id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ResourceId Id<T>() => ResourceIdInteral<T>.Id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in ResourceId l, in ResourceId r) => l._id == r._id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in ResourceId l, in ResourceId r) => l._id != r._id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator uint(in ResourceId resourceId) => resourceId._id;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator int(in ResourceId resourceId) => unchecked((int)resourceId._id);
    public override bool Equals(object obj) => throw new NotSupportedException("Use == to avoid boxing");
    public override int GetHashCode() => (int)_id;

#if DEBUG
    private static readonly Dictionary<uint, string> _resourceNames = new();
    public override string ToString() => _resourceNames.TryGetValue(_id, out var id) ? id : _id.ToString();
#else
    public override string ToString() => _id.ToString();
#endif
    private readonly struct ResourceIdInteral<T>
    {
        public static readonly ResourceId Id = CreateNew();
        private static ResourceId CreateNew()
        {
            var id = new ResourceId(IdGenerator<ResourceId>.Next());
#if DEBUG
            _resourceNames[id] = typeof(T).FormattedName();
#endif
            return id;
        }
    }
}
