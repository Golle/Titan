using Titan.Graphics.Resources;
using Titan.Graphics.Textures;

namespace Titan.EntitySystem.Components
{

    internal struct MeshComponent
    {
        // TODO: handle "submesh", should probably be loaded as different entities
        public Handle<VertexBuffer> VertexBuffer;
        public Handle<IndexBuffer> IndexBuffer;
    }
    
    internal struct TextureComponent
    {
        public Texture Texture;
    }
}
