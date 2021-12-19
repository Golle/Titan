using Titan.Graphics;
using Titan.Graphics.D3D11;

namespace Titan.Rendering.Sprites;

internal class SpriteRenderer : Renderer
{
    private readonly SpriteRenderQueue _queue;

    public SpriteRenderer(SpriteRenderQueue queue)
    {
        _queue = queue;
    }
    public override void Render(Context context)
    {
        

    }
}
