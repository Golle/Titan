namespace Titan.Platform.Win32.D3D12;

public struct D3D12_RASTERIZER_DESC
{
    public D3D12_FILL_MODE FillMode;
    public D3D12_CULL_MODE CullMode;
    public int FrontCounterClockwise;// unmanaged bool
    public int DepthBias;
    public float DepthBiasClamp;
    public float SlopeScaledDepthBias;
    public int DepthClipEnable; // unmanaged bool
    public int MultisampleEnable;// unmanaged bool
    public int AntialiasedLineEnable;// unmanaged bool
    public uint ForcedSampleCount;
    public D3D12_CONSERVATIVE_RASTERIZATION_MODE ConservativeRaster;


    public static D3D12_RASTERIZER_DESC Default() =>
        new()
        {
            FillMode = D3D12_FILL_MODE.D3D12_FILL_MODE_SOLID,
            CullMode = D3D12_CULL_MODE.D3D12_CULL_MODE_BACK,
            FrontCounterClockwise = 0,
            DepthBias = 0, //D3D12_DEFAULT_DEPTH_BIAS, 
            DepthBiasClamp = 0, //D3D12_DEFAULT_DEPTH_BIAS_CLAMP,  
            SlopeScaledDepthBias = 0, //D3D12_DEFAULT_SLOPE_SCALED_DEPTH_BIAS
            DepthClipEnable = 1,
            MultisampleEnable = 0,
            AntialiasedLineEnable = 0,
            ForcedSampleCount = 0,
            ConservativeRaster = D3D12_CONSERVATIVE_RASTERIZATION_MODE.D3D12_CONSERVATIVE_RASTERIZATION_MODE_OFF
        };
}
