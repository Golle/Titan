using System.Runtime.CompilerServices;

namespace Titan.Platform.Win32.D3D12;

public struct D3D12_BLEND_DESC
{
    public int AlphaToCoverageEnable; // Unmanaged BOOL
    public int IndependentBlendEnable; // Unmanaged BOOL
    private D3D12_BLEND_DESC_RENDER_TARGETS _renderTargets;

    /// <summary>
    /// Maximum of 8 render targets.
    /// </summary>
    public unsafe D3D12_RENDER_TARGET_BLEND_DESC* RenderTarget => (D3D12_RENDER_TARGET_BLEND_DESC*)Unsafe.AsPointer(ref _renderTargets);
    private struct D3D12_BLEND_DESC_RENDER_TARGETS
    {
        public D3D12_RENDER_TARGET_BLEND_DESC RenderTarget1, RenderTarget2, RenderTarget3, RenderTarget4, RenderTarget5, RenderTarget6, RenderTarget7, RenderTarget8;
    }
    public static D3D12_BLEND_DESC Default() =>
        new()
        {
            AlphaToCoverageEnable = 0,
            IndependentBlendEnable = 0,
            _renderTargets =
            {
                RenderTarget1 = new()
                {
                    BlendEnable = 0,
                    SrcBlend = D3D12_BLEND.D3D12_BLEND_ONE,
                    DestBlend = D3D12_BLEND.D3D12_BLEND_ZERO,
                    BlendOp = D3D12_BLEND_OP.D3D12_BLEND_OP_ADD,
                    SrcBlendAlpha = D3D12_BLEND.D3D12_BLEND_ONE,
                    DestBlendAlpha = D3D12_BLEND.D3D12_BLEND_ZERO,
                    BlendOpAlpha = D3D12_BLEND_OP.D3D12_BLEND_OP_ADD,
                    LogicOp = D3D12_LOGIC_OP.D3D12_LOGIC_OP_NOOP,
                    RenderTargetWriteMask = D3D12_COLOR_WRITE_ENABLE.D3D12_COLOR_WRITE_ENABLE_ALL
                }
            }
        };
}