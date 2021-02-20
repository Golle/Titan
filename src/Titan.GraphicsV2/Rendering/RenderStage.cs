using System;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.Rendering.Commands;
using static Titan.GraphicsV2.Rendering.Commands.RenderCommandTypes;

namespace Titan.GraphicsV2.Rendering
{
    internal class RenderStage
    {
        private readonly CommandBuffer _commands;
        public RenderStage(CommandBuffer commands)
        {
            _commands = commands;
        }
        
        public unsafe void Render(Context context)
        {
            var commands = _commands.Enumerate();
            RenderCommandTypes type;
            do
            {
                switch (type = commands.NextType)
                {
                    case SetRenderTarget:
                        var renderTarget = commands.GetAndMoveToNext<SetRenderTargetsCommand>();
                        context.SetRenderTargets(renderTarget->NumberOfTargets, renderTarget->RenderTargets, null);
                        break;
                    
                    case SetPixelShaderResource:
                        var psResources = commands.GetAndMoveToNext<SetPixelShaderResourcesCommand>();
                        context.SetPixelShaderResources(psResources->NumberOfViews, psResources->Resources);
                        break;
                    
                    case SetVertexShaderResource:
                        var vsResources = commands.GetAndMoveToNext<SetVertexShaderResourcesCommand>();
                        context.SetVertexShaderResources(vsResources->NumberOfViews, vsResources->Resources);
                        break;

                    case ClearRenderTarget:
                        var clear = commands.GetAndMoveToNext<ClearRenderTargetCommand>();
                        context.ClearRenderTarget(clear->RenderTarget, clear->Color);
                        break;
                    case Invalid:
                        break;
#if DEBUG
                    default:
                        throw new ArgumentOutOfRangeException(nameof(commands.NextType), type, "All commands must be handled");
#endif
                }
            } while (type != Invalid);
        }
    }
}
