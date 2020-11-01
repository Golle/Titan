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
            foreach (var command in _commands)
            {
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
                    default:
                        throw new InvalidOperationException("Render command not found.");
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static unsafe void SetRenderTargets(IRenderContext context, in SetMultipleRenderTargetViewCommand command)
        {
            fixed (ulong* pRenderTargets = command.Pointers)
            {
                context.SetRenderTargets((ID3D11RenderTargetView**)&pRenderTargets, command.Count, command.DepthStencilView);
            }
        }
    }
}
