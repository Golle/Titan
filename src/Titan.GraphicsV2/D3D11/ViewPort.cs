using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{

    // TODO: figure out how to use this in the renderer
    internal struct ViewPort
    {
        internal D3D11_VIEWPORT Resource;
        public ViewPort(int width, int height, int topLeftX = 0, int topLeftY = 0, float minDepth = 0f, float maxDepth = 1f)
        {
            Resource.Width = width;
            Resource.Height = height;
            Resource.TopLeftX = topLeftX;
            Resource.TopLeftY = topLeftY;
            Resource.MinDepth = minDepth;
            Resource.MaxDepth = maxDepth;
        }
    }
}
