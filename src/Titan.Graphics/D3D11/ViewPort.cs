using Titan.Windows.D3D11;

namespace Titan.Graphics.D3D11
{
    public struct ViewPort
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
