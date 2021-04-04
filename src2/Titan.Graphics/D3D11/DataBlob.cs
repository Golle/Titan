using System.Runtime.CompilerServices;

namespace Titan.Graphics.D3D11
{
    public readonly unsafe struct DataBlob
    {
        private readonly void* _data;
        private readonly uint _size;
        public DataBlob(void* data, uint size = 0u)
        {
            _data = data;
            _size = size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator void*(in DataBlob blob) => blob._data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator DataBlob(in void* data) => new(data);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasValue() => _data != null;

        public uint Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _size;
        }
    }
}
