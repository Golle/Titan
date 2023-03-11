namespace Titan.Graphics;

internal interface ISwapChain
{
    void Present();
    void Resize(uint width, uint height);
    void ToggleFullscreen();
}
