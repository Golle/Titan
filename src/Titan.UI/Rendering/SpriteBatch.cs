using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Memory;
using Titan.Graphics;
using Titan.UI.Common;

namespace Titan.UI.Rendering
{
    internal unsafe class SpriteBatch : IDisposable
    {
        private readonly MemoryChunk<NormalSprite> _sprites;
        private int _count;
        public SpriteBatch(uint maxSprites)
        {
            _sprites = MemoryUtils.AllocateBlock<NormalSprite>(maxSprites);
        }

        public int Add(in Vector2 position, in Size size, in Color color, ReadOnlySpan<Vector2> coordinates)
        {
            var index = NextIndex();
            var sprite = _sprites.GetPointer(index);
            fixed (Vector2* pCoordinates = coordinates)
            {
                Buffer.MemoryCopy(pCoordinates, &sprite->Coordinates, 32, 32);
            }
            sprite->Position = position;
            sprite->Size = size;
            sprite->Color = color;
            return index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining|MethodImplOptions.AggressiveOptimization)]
        public void Render(int index, ref UIVertex* vertex)
        {
            var sprite = _sprites.GetPointer(index);

            var size = sprite->Size;
            var position = sprite->Position;
            var top = position.Y + size.Height;
            var right = position.X + size.Width;

            vertex->Position = position;
            vertex->Texture = sprite->Coordinates[0];
            vertex->Color = sprite->Color;

            vertex++;
            vertex->Position = new Vector2(position.X, top);
            vertex->Texture = sprite->Coordinates[1];
            vertex->Color = sprite->Color;

            vertex++;
            vertex->Position = new Vector2(right, top);
            vertex->Texture = sprite->Coordinates[2];
            vertex->Color = sprite->Color;

            vertex++;
            vertex->Position = new Vector2(right, position.Y);
            vertex->Texture = sprite->Coordinates[3];
            vertex->Color = sprite->Color;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int NextIndex() => Interlocked.Increment(ref _count) - 1;

        public void Clear()
        {
            _count = 0;
        }
        public void Dispose()
        {
            _sprites.Free();
        }
    }
}
