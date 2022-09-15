namespace Titan.Graphics.D3D11.Textures
{
    [Flags]
    public enum TextureBindFlags
    {
        None = 0,
        ShaderResource = 1,
        RenderTarget = 2,
        DepthBuffer = 4,
        UnorderedAccess = 8,
        FrameBuffer = ShaderResource | RenderTarget
    }
}
