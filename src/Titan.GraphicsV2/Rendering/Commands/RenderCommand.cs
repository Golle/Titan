namespace Titan.GraphicsV2.Rendering.Commands
{
    internal struct RenderCommand
    {
        internal RenderCommandTypes Type;

        internal static RenderCommand Default = new() {Type = RenderCommandTypes.Render };
    }
}
