using Titan.Core;
using Titan.ECS.Components;
using Titan.Graphics.Resources;

namespace Titan.BuiltIn.Components;

/// <summary>
/// Internal representation of a texture, this will be read by other systems(for example Sprite Renderer) when something is rendered.
/// </summary>
internal struct TextureComponent : IComponent
{
    public Handle<Texture> TextureHandle;
    internal uint TextureId;
    internal uint Height;
    internal uint Width;
}
