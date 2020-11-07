using System;
using System.Runtime.CompilerServices;
using Titan.Graphics.D3D11;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Pipeline
{
    internal class RenderPass
    {
        private readonly string _name;
        private readonly RenderPassCommand[] _commands;

        public RenderPass(string name, RenderPassCommand[] commands)
        {
            _name = name;
            _commands = commands;
        }

        public void Render(IRenderContext renderContext)
        {
            // TODO: replace the generation of this with a SourceGenerator (C# 9) - not good since the engine needs to be re-compiled if we change a variable.
            // TODO: Use dynamic methods for this instead, create the IL code. 

            for (var i = 0; i < _commands.Length; i++)
            {
                ref readonly var command = ref _commands[i];

                switch (command.Type)
                {
                    case CommandType.ClearRenderTarget:
                        renderContext.ClearRenderTargetView(command.ClearRenderTarget.RenderTarget, command.ClearRenderTarget.Color);
                        break;
                    case CommandType.ClearDepthStencil:
                        renderContext.ClearDepthStencilView(command.DepthStencil);
                        break;
                    case CommandType.SetRenderTargetAndDepthStencil:
                        renderContext.SetRenderTarget(command.RenderTarget, command.DepthStencil);
                        break;
                    case CommandType.SetRenderTarget:
                        renderContext.SetRenderTarget(command.RenderTarget);
                        break;
                    case CommandType.SetMultipleRenderTarget:
                        SetRenderTargets(renderContext, command.MultipleRenderTargets);
                        break;
                    case CommandType.SetShaderProgram:
                        command.ShaderProgram.Bind(renderContext);
                        break;
                    case CommandType.Render:
                        command.Renderer.Render(renderContext);
                        break;
                    case CommandType.SetVertexShaderResource:
                        renderContext.SetVertexShaderResource(command.ShaderResource.View, command.ShaderResource.Slot);
                        break;
                    case CommandType.SetPixelShaderResource:
                        renderContext.SetPixelShaderResource(command.ShaderResource.View, command.ShaderResource.Slot);
                        break;
                    case CommandType.SetVertexShaderSampler:
                        renderContext.SetVertexShaderSampler(command.SamplerState.Sampler, command.SamplerState.Slot);
                        break;
                    case CommandType.SetPixelShaderSampler:
                        renderContext.SetPixelShaderSampler(command.SamplerState.Sampler, command.SamplerState.Slot);
                        break;
                    case CommandType.UnbindRenderTargets:
                        UnbindRenderTargets(renderContext);
                        break;
                    case CommandType.UnbindPixelShaderResources:
                        UnbindPixelShaderResources(renderContext, command.Count);
                        break;
                    case CommandType.UnbindVertexShaderResources:
                        UnbindVertexShaderResources(renderContext, command.Count);
                        break;
                    default:
                        throw new InvalidOperationException("Render command not found.");
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void UnbindVertexShaderResources(IRenderContext context, uint numViews)
        {
            var resources = stackalloc ID3D11ShaderResourceView*[(int)numViews];
            context.SetVertexShaderResources(resources, numViews, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void UnbindPixelShaderResources(IRenderContext context, uint numViews)
        {
            var resources = stackalloc ID3D11ShaderResourceView*[(int)numViews];
            context.SetPixelShaderResources(resources, numViews, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void  UnbindRenderTargets(IRenderContext context)
        {
            var renderTargets = stackalloc ID3D11RenderTargetView*[1];
            context.SetRenderTargets(renderTargets, 1, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static unsafe void SetRenderTargets(IRenderContext context, in SetMultipleRenderTargetViewCommand command)
        {
            fixed (ulong* pRenderTargets = command.Pointers)
            {
                context.SetRenderTargets((ID3D11RenderTargetView**)pRenderTargets, command.Count, command.DepthStencilView);
            }
        }
    }
}
