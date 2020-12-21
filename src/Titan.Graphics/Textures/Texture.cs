using Titan.Graphics.Resources;

namespace Titan.Graphics.Textures
{
    public record Texture(Handle<Resources.Texture> TextureHandle, Handle<ShaderResourceView> ResourceViewHandle);
}
