using Titan.Graphics.D3D11;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.D3D11.D3D11_BIND_FLAG;
using static Titan.Windows.Win32.D3D11.DXGI_FORMAT;
using static Titan.Windows.Win32.D3D11.D3D_PRIMITIVE_TOPOLOGY;

namespace Titan.Graphics.Pipeline.Graph
{


    public class GBufferRenderPass : IRenderPass
    {
        private readonly IMeshRenderQueue _meshRenderQueue;
        
        private readonly RenderBuffer _normalsBuffer;
        private readonly RenderBuffer _albedoBuffer;

        private readonly DepthStencilView _depthStencil;

        private readonly Color _clearColor = Color.Black;

        public GBufferRenderPass(IMeshRenderQueue meshRenderQueue, IBufferManager bufferManager)
        {
            _meshRenderQueue = meshRenderQueue;
            _normalsBuffer = bufferManager.GetBuffer(DXGI_FORMAT_R8G8B8A8_UNORM, D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE);
            _albedoBuffer = bufferManager.GetBuffer(DXGI_FORMAT_R8G8B8A8_UNORM, D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE);

            _depthStencil = bufferManager.GetDepthStencil();

        }


        public void Begin(DeferredContext context)
        {
            context.ClearRenderTargetView(_normalsBuffer.RenderTargetView, _clearColor);
            context.ClearRenderTargetView(_albedoBuffer.RenderTargetView, _clearColor);
            
            unsafe
            {
                var renderTargets = stackalloc ID3D11RenderTargetView*[2];
                renderTargets[0] = _normalsBuffer.RenderTargetView.AsPointer();
                renderTargets[1] = _albedoBuffer.RenderTargetView.AsPointer();
                context.AsPointer()->OMSetRenderTargets(2, renderTargets, _depthStencil.AsPointer());
            }
            //context.SetPritimiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            

        }

        public void Render(DeferredContext context)
        {

            foreach (ref readonly var renderable in _meshRenderQueue.GetRenderables())
            {
                
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
}
