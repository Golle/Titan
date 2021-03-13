using System.Runtime.CompilerServices;

namespace Titan.GraphicsV2.Rendering.Commands
{
    internal unsafe ref struct CommandBufferEnumerator
    {
        private readonly byte* _end;
        private byte* _current;
        public CommandBufferEnumerator(byte* buffer, int size)
        {
            _current = buffer;
            _end = buffer + size;
        }

        internal RenderCommandTypes NextType
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _current >= _end ? RenderCommandTypes.Invalid : *(RenderCommandTypes*) _current;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal T* GetAndMoveToNext<T>() where T : unmanaged
        {
            var current = _current;
            _current += sizeof(T);
            return (T*)current;
        }
    }
}
