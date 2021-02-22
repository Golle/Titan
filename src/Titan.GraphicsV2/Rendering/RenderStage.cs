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
            // TODO, can this be used to create some IL code instead of using a loop?

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
                    
                    case SetPixelShaderSamplers:
                        var pixelShaderSampler = commands.GetAndMoveToNext<SetPixelShaderSamplersCommand>();
                        context.SetPixelShaderSamplers(pixelShaderSampler->NumberOfSamplers, pixelShaderSampler->Samplers);
                        break;
                    
                    case SetVertexShaderSamplers:
                        var vertexShaderSampler = commands.GetAndMoveToNext<SetVertexShaderSamplersCommand>();
                        context.SetVertexShaderSamplers(vertexShaderSampler->NumberOfSamplers, vertexShaderSampler->Samplers);
                        break;

                    case SetShaders:
                        var shaders = commands.GetAndMoveToNext<SetShadersCommand>();
                        context.SetPixelShader(shaders->PixelShader);
                        context.SetVertexShader(shaders->VertexShader);
                        context.SetInputLayout(shaders->InputLayout);
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
