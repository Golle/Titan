namespace Titan.Graphics.D3D11
{
    public unsafe class BackBufferRenderTargetView : RenderTargetView
    {
        public BackBufferRenderTargetView(IGraphicsDevice device) 
            : base(device.BackBuffer.Get())
        {
        }
    }
}
