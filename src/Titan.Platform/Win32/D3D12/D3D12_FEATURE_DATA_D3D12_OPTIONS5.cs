using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.D3D12;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct D3D12_FEATURE_DATA_D3D12_OPTIONS5
{
    public int SRVOnlyTiledResourceTier3; // bool
    public D3D12_RENDER_PASS_TIER RenderPassesTier;
    public D3D12_RAYTRACING_TIER RaytracingTier;
}
