using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Graphics.D3D11.Textures;
using Titan.Graphics.Loaders.Atlas;
using Titan.UI.Common;

namespace Titan.UI.Rendering
{
    [SkipLocalsInit]
    internal struct QueuedRenderable
    {
        public Vector2 Position;
        public Size Size;
        public Handle<Texture> Texture;
        public TextureCoordinates Coordinates;
    }
}
