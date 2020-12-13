using System;
using System.Runtime.CompilerServices;
using Titan.Graphics.D3D11;
using Titan.Graphics.Resources;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Pipeline
{
    internal class RenderPass
    {
        private readonly string _name;
        private readonly RenderPassCommand[] _commands;
        private readonly IGraphicsDevice _device;
        private readonly IShaderResourceViewManager _shaderResourceViewManager;

        public RenderPass(string name, RenderPassCommand[] commands, IGraphicsDevice device, IShaderResourceViewManager shaderResourceViewManager)
        {
            _name = name;
            _commands = commands;
            _device = device;
            _shaderResourceViewManager = shaderResourceViewManager;
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
                        renderContext.ClearRenderTargetView(_device.RenderTargetViewManager[command.ClearRenderTarget.RenderTarget], command.ClearRenderTarget.Color);
                        break;
                    case CommandType.ClearDepthStencil:
                        renderContext.ClearDepthStencilView(_device.DepthStencilViewManager[command.DepthStencil]);
                        break;
                    case CommandType.SetRenderTargetAndDepthStencil:
                        renderContext.SetRenderTarget(_device.RenderTargetViewManager[command.RenderTarget], _device.DepthStencilViewManager[command.DepthStencil]);
                        break;
                    case CommandType.SetRenderTarget:
                        renderContext.SetRenderTarget(_device.RenderTargetViewManager[command.RenderTarget]);
                        break;
                    case CommandType.SetMultipleRenderTargetAndDepthStencil:
                        SetRenderTargets(renderContext, command.MultipleRenderTargets, true);
                        break;
                    case CommandType.SetMultipleRenderTargets:
                        SetRenderTargets(renderContext, command.MultipleRenderTargets);
                        break;
                    case CommandType.SetShaderProgram:
                        var shader = command.ShaderProgram;
                        renderContext.SetInputLayout(_device.ShaderManager[shader.InputLayout]);
                        renderContext.SetPixelShader(_device.ShaderManager[shader.PixelShader]);
                        renderContext.SetVertexShader(_device.ShaderManager[shader.VertexShader]);
                        break;
                    case CommandType.Render:
                        command.Renderer.Render(renderContext);
                        break;
                    case CommandType.SetVertexShaderResource:
                        renderContext.SetVertexShaderResource(_shaderResourceViewManager[command.ShaderResource.Handle], command.ShaderResource.Slot);
                        break;
                    case CommandType.SetPixelShaderResource:
                        renderContext.SetPixelShaderResource(_shaderResourceViewManager[command.ShaderResource.Handle], command.ShaderResource.Slot);
                        break;
                    case CommandType.SetVertexShaderSampler:
                        renderContext.SetVertexShaderSampler(_device.SamplerStateManager[command.SamplerState.Sampler], command.SamplerState.Slot);
                        break;
                    case CommandType.SetPixelShaderSampler:
                        renderContext.SetPixelShaderSampler(_device.SamplerStateManager[command.SamplerState.Sampler], command.SamplerState.Slot);
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
        private unsafe void SetRenderTargets(IRenderContext context, in SetMultipleRenderTargetViewCommand command, bool hasDepthStencil = false)
        {
            var renderTargets = stackalloc ID3D11RenderTargetView*[(int)command.Count];
            for (var i = 0; i < command.Count; ++i)
            {
                renderTargets[i] = _device.RenderTargetViewManager[command.Handles[i]].Pointer;
            }

            var depthStencilView = hasDepthStencil ? _device.DepthStencilViewManager[command.DepthStencilView].Pointer : null;
            context.SetRenderTargets(renderTargets, command.Count, depthStencilView);
        }
    }
}
