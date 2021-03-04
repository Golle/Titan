using System;
using System.Runtime.CompilerServices;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.Rendering.Commands;
using Titan.GraphicsV2.Rendering.Renderers;
using Titan.Windows.Win32.D3D11;
using static Titan.GraphicsV2.Rendering.Commands.RenderCommandTypes;

namespace Titan.GraphicsV2.Rendering.Pipepline
{
    internal class RenderStage : IDisposable
    {
        private readonly CommandBuffer _commands;
        private readonly IRenderer _renderer;

        internal RenderStage(CommandBuffer commands, IRenderer renderer)
        {
            _commands = commands;
            _renderer = renderer;
        }


        public unsafe void Render(Context context)
        {
            // TODO, can this be used to create some IL code instead of using a loop?

            var emptyRenderTarget = stackalloc ID3D11RenderTargetView*[1];

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

                    case UnbindRenderTargets:
                        _ = commands.GetAndMoveToNext<UnbindPixelShaderResourcesCommand>();
                        context.SetRenderTargets(1, emptyRenderTarget, null);
                        break;

                    case UnbindPixelShaderResources:
                    {
                        var command = commands.GetAndMoveToNext<UnbindPixelShaderResourcesCommand>();
                        UnbindPixelShaderResource(context, command->Count);
                        break;
                    }

                    case UnbindVertexShaderResources:
                    {
                        var command = commands.GetAndMoveToNext<UnbindPixelShaderResourcesCommand>();
                        UnbindVertexShaderResource(context, command->Count);
                        break;
                    }
                    case RenderCommandTypes.Render:
                        _renderer.Render(context);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void UnbindVertexShaderResource(Context context, int count)
        {
            var resources = stackalloc ID3D11ShaderResourceView*[count];
            context.SetVertexShaderResources((uint)count, resources);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void UnbindPixelShaderResource(Context context, int count)
        {
            var resources = stackalloc ID3D11ShaderResourceView*[count];
            context.SetPixelShaderResources((uint) count, resources);
        }

        public void Dispose()
        {
            _commands.Dispose();
            _renderer.Dispose();
        }
    }
}
