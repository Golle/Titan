using Titan.Graphics.Resources;

namespace Titan.EntitySystem.Components
{

    internal struct MeshComponent
    {
        // TODO: handle "submesh", should probably be loaded as different entities
        public VertexBufferHandle VertexBufferHandle;
        public IndexBufferHandle IndexBufferHandle;
    }
    
    internal struct TextureComponent
    {
        public TextureHandle Handle;
    }
}
