using System.Runtime.InteropServices;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.Shaders;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.D3D11.D3D11_BIND_FLAG;
using static Titan.Windows.Win32.D3D11.DXGI_FORMAT;
using static Titan.Windows.Win32.D3D11.D3D_PRIMITIVE_TOPOLOGY;

namespace Titan.Graphics.Pipeline.Graph
{


    

    public class GBufferRenderPass : IRenderPass
    {
        private readonly IMeshRenderQueue _meshRenderQueue;

        public ShaderResourceView NormalResourceView => _normalsBuffer.ShaderResourceView;
        public ShaderResourceView AlbedoResourceView => _albedoBuffer.ShaderResourceView;
        public ShaderResourceView DepthBufferView => _depthStencil.ResourceView;

        private readonly RenderBuffer _normalsBuffer;
        private readonly RenderBuffer _albedoBuffer;

        private readonly DepthStencil _depthStencil;

        private readonly Color _clearColor = Color.Black;
        
        private readonly ShaderProgram _program;

        public GBufferRenderPass(IMeshRenderQueue meshRenderQueue, IBufferManager bufferManager, IShaderManager shaderManager)
        {
            _meshRenderQueue = meshRenderQueue;
            _normalsBuffer = bufferManager.GetBuffer(DXGI_FORMAT_R8G8B8A8_UNORM, D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE);
            _albedoBuffer = bufferManager.GetBuffer(DXGI_FORMAT_R8G8B8A8_UNORM, D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE);
            
            _depthStencil = bufferManager.GetDepthStencil(bindFlag: D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_DEPTH_STENCIL, shaderResourceFormat: DXGI_FORMAT_R24_UNORM_X8_TYPELESS);

            _program = shaderManager.Get(shaderManager.GetHandle("GBufferDefault"));
        }


        public void Begin(ImmediateContext context)
        {
            context.ClearRenderTargetView(_normalsBuffer.RenderTargetView, _clearColor);
            context.ClearRenderTargetView(_albedoBuffer.RenderTargetView, _clearColor);
            context.ClearDepthStencilView(_depthStencil.View);
            
            unsafe
            {
                var renderTargets = stackalloc ID3D11RenderTargetView*[2];
                renderTargets[0] = _albedoBuffer.RenderTargetView.AsPointer();
                renderTargets[1] = _normalsBuffer.RenderTargetView.AsPointer();
                context.AsPointer()->OMSetRenderTargets(2, renderTargets, _depthStencil.View.AsPointer());
            }

            // This wont work when we have different types of geometry
            context.SetPritimiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            _program.Bind(context);
            //context.SetPixelShader(_pixelShader);
            //context.SetVertexShader(_vertexShader);
            //context.SetInputLayout(_inputLayout);
        }

        public void Render(ImmediateContext context)
        {
            foreach (ref readonly var renderable in _meshRenderQueue.GetRenderables())
            {
                var (vertexBuffer, indexBuffer, subSets) = renderable.Mesh;
                
                context.SetVertexBuffer(vertexBuffer);
                context.SetIndexBuffer(indexBuffer);
                if (subSets.Length > 0)
                {
                    for (var i = 0; i < subSets.Length; ++i)
                    {
                        ref readonly var subset = ref subSets[i];
                        context.DrawIndexed((uint) subset.Count, (uint) subset.StartIndex);
                    }
                }
                else
                {
                    context.DrawIndexed(indexBuffer.NumberOfIndices);
                }
            }
        }

        public CommandList End(DeferredContext context)
        {
            return context.FinishCommandList();
        }


        public void Dispose()
        {
            _normalsBuffer.Dispose();
            _albedoBuffer.Dispose();
            _depthStencil.Dispose();
        }
    }


    public enum CommandType : byte
    {
        SetVertexBuffer,
        SetIndexBuffer,
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct DrawCommand
    {
        [FieldOffset(0)]
        public readonly CommandType Type;
        [FieldOffset(sizeof(CommandType))]
        public SetVertexBufferCommand VertexBuffer;
        [FieldOffset(sizeof(CommandType))]
        public SetVertexBufferCommand IndexBuffer;


    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SetVertexBufferCommand
    {
        public uint BufferId;
        public uint Offset;
    }
}

