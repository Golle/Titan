using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Memory;
using Titan.Graphics.Loaders.Atlas;

namespace Titan.Graphics.Rendering.Sprites
{
    internal unsafe class NineSliceSpriteBatch : IDisposable
    {
        private readonly MemoryChunk<NineSliceSprite> _sprites;
        private int _count;
        public NineSliceSpriteBatch(uint maxSprites)
        {
            _sprites = MemoryUtils.AllocateBlock<NineSliceSprite>(maxSprites);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AddNineSlice(in Vector2 position, in Size size, ReadOnlySpan<Vector2> coordinates, in Color color, in Margins margins)
        {
            var index = NextIndex();
            var renderable = _sprites.GetPointer(index);
            fixed (Vector2* pCoordinates = coordinates)
            {
                Buffer.MemoryCopy(pCoordinates, &renderable->Coordinates, 128, 128);
            }
            renderable->Position = position;
            renderable->Size = size;
            renderable->Color = color;
            renderable->Margins = margins;
            return index;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Render(int index, ref SpriteVertex* vertex)
        {
            var renderable = _sprites.GetPointer(index);
            var size = renderable->Size;
            var position = renderable->Position;
            var margins = renderable->Margins;
            var color = renderable->Color;

            var positions = stackalloc Vector2[4];
            positions[0] = position;
            positions[1] = new Vector2(positions->X + margins.Left, positions->Y + margins.Bottom);
            positions[2] = new Vector2(positions->X + size.Width - margins.Right, positions->Y + size.Height - margins.Top);
            positions[3] = new Vector2(positions->X + size.Width, positions->Y + size.Height);

            // TODO: compare this with updating and uploading Indices. This creates 36 vertices, but only 16 are required. But if we use 16 we need to update the indices on each loop.
            for (var row = 0; row < 3; ++row)
            {
                for (var col = 0; col < 3; ++col)
                {
                    var textureOffset = row * 4 + col;

                    vertex->Position = new Vector2(positions[col].X, positions[row].Y);
                    vertex->Texture = renderable->Coordinates[textureOffset];
                    vertex->Color = color;

                    vertex++;
                    vertex->Position = new Vector2(positions[col].X, positions[row + 1].Y);
                    vertex->Texture = renderable->Coordinates[textureOffset + 4];
                    vertex->Color = color;

                    vertex++;
                    vertex->Position = new Vector2(positions[col + 1].X, positions[row + 1].Y);
                    vertex->Texture = renderable->Coordinates[textureOffset + 5];
                    vertex->Color = color;

                    vertex++;
                    vertex->Position = new Vector2(positions[col + 1].X, positions[row].Y);
                    vertex->Texture = renderable->Coordinates[textureOffset + 1];
                    vertex->Color = color;
                    vertex++;
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int NextIndex() => Interlocked.Increment(ref _count) - 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
