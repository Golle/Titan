using System.Runtime.CompilerServices;
using Titan.Core;

namespace Titan.Assets
{
    public unsafe struct AssetComponent<T>
    {
        public fixed char Id[128];
        public Handle<Asset> AssetHandle;
        public Handle<T> Handle;
        public T DefaultValue;
        public AssetComponent(string id, in T defaultValue = default)
        {
            fixed (char* destination = Id)
            fixed (char* source = id)
            {
                Unsafe.CopyBlock(destination, source, (uint)(id.Length * sizeof(char)));
            }
            AssetHandle = default;
            Handle = default;
            DefaultValue = defaultValue;
        }

        public override string ToString()
        {
            fixed (char* pId = Id)
            {
                return new(pId);
            }
        }
    }
}
