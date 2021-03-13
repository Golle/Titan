namespace Titan.GraphicsV2.Rendering.Commands
{
    internal struct UnbindVertexShaderResourcesCommand
    {
        internal RenderCommandTypes Type;
        internal int Count;

        public UnbindVertexShaderResourcesCommand(int count)
        {
            Type = RenderCommandTypes.UnbindVertexShaderResources;
            Count = count;
        }
    }

    internal struct UnbindPixelShaderResourcesCommand
    {
        internal RenderCommandTypes Type;
        internal int Count;

        public UnbindPixelShaderResourcesCommand(int count)
        {
            Type = RenderCommandTypes.UnbindPixelShaderResources;
            Count = count;
        }
    }

    internal struct UnbindRenderTargetsCommand
    {
        internal RenderCommandTypes Type;
        internal int Count;

        public UnbindRenderTargetsCommand(int count)
        {
            Type = RenderCommandTypes.UnbindRenderTargets;
            Count = count;
        }
    }
}
