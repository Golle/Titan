using System.Diagnostics;
using Titan.Graphics.Resources;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D12;
using static Titan.Platform.Win32.D3D12.D3D12_COMPARISON_FUNC;
using static Titan.Platform.Win32.D3D12.D3D12_FILTER;
using static Titan.Platform.Win32.D3D12.D3D12_STATIC_BORDER_COLOR;
using static Titan.Platform.Win32.D3D12.D3D12_TEXTURE_ADDRESS_MODE;
using static Titan.Platform.Win32.D3D12.D3D12Constants;

namespace Titan.Graphics.D3D12.Utils;

//NOTE(Jens): add more if needed

internal static class D3D12Helpers
{

    private static readonly D3D12_BLEND_DESC[] BlendStateDescs = new D3D12_BLEND_DESC[(int)BlendStateType.Count];

    static unsafe D3D12Helpers()
    {
        // set up the blend states

        {
            ref var blendDesc = ref BlendStateDescs[(int)BlendStateType.Disabled];
            blendDesc.RenderTarget[0].BlendEnable = 0;
            blendDesc.RenderTarget[0].BlendOp = D3D12_BLEND_OP.D3D12_BLEND_OP_ADD;
            blendDesc.RenderTarget[0].BlendOpAlpha = D3D12_BLEND_OP.D3D12_BLEND_OP_ADD;
            blendDesc.RenderTarget[0].DestBlend = D3D12_BLEND.D3D12_BLEND_INV_SRC_ALPHA;
            blendDesc.RenderTarget[0].DestBlendAlpha = D3D12_BLEND.D3D12_BLEND_ONE;
            blendDesc.RenderTarget[0].RenderTargetWriteMask = D3D12_COLOR_WRITE_ENABLE.D3D12_COLOR_WRITE_ENABLE_ALL;
            blendDesc.RenderTarget[0].SrcBlend = D3D12_BLEND.D3D12_BLEND_SRC_ALPHA;
            blendDesc.RenderTarget[0].SrcBlendAlpha = D3D12_BLEND.D3D12_BLEND_ONE;
        }
        {
            ref var blendDesc = ref BlendStateDescs[(int)BlendStateType.AlphaBlend];
            blendDesc.RenderTarget[0].BlendEnable = 1;
            blendDesc.RenderTarget[0].BlendOp = D3D12_BLEND_OP.D3D12_BLEND_OP_ADD;
            blendDesc.RenderTarget[0].BlendOpAlpha = D3D12_BLEND_OP.D3D12_BLEND_OP_ADD;
            blendDesc.RenderTarget[0].DestBlend = D3D12_BLEND.D3D12_BLEND_INV_SRC_ALPHA;
            blendDesc.RenderTarget[0].DestBlendAlpha = D3D12_BLEND.D3D12_BLEND_ONE;
            blendDesc.RenderTarget[0].RenderTargetWriteMask = D3D12_COLOR_WRITE_ENABLE.D3D12_COLOR_WRITE_ENABLE_ALL;
            blendDesc.RenderTarget[0].SrcBlend = D3D12_BLEND.D3D12_BLEND_SRC_ALPHA;
            blendDesc.RenderTarget[0].SrcBlendAlpha = D3D12_BLEND.D3D12_BLEND_ONE;
        }
#if DEBUG
        Debug.Assert(Enum.GetValues(typeof(BlendStateType)).Length == 3, "Added new blend state descs but didn't update this code.");
#endif
    }

    public static D3D12_BLEND_DESC GetBlendState(BlendStateType type)
    {
        Debug.Assert((int)type <= BlendStateDescs.Length && (int)type >= 0);
        return BlendStateDescs[(int)type];
    }


    public static void InitDescriptorRanges(Span<D3D12_DESCRIPTOR_RANGE1> ranges, D3D12_DESCRIPTOR_RANGE_TYPE type)
    {
        for (var i = 0; i < ranges.Length; ++i)
        {
            ranges[i] = new D3D12_DESCRIPTOR_RANGE1
            {
                BaseShaderRegister = 0,
                Flags = D3D12_DESCRIPTOR_RANGE_FLAGS.D3D12_DESCRIPTOR_RANGE_FLAG_DESCRIPTORS_VOLATILE,
                NumDescriptors = D3D12_DESCRIPTOR_RANGE_OFFSET_APPEND,
                OffsetInDescriptorsFromTableStart = 0,
                RangeType = type,
                RegisterSpace = (uint)i
            };
        }
    }

    public static D3D12_STATIC_SAMPLER_DESC CreateStaticSamplerDesc(SampleState state, uint register, uint registerSpace, D3D12_SHADER_VISIBILITY visibiliy = D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_ALL) =>
        state switch
        {
            SampleState.Linear => new D3D12_STATIC_SAMPLER_DESC
            {
                AddressU = D3D12_TEXTURE_ADDRESS_MODE_CLAMP,
                AddressV = D3D12_TEXTURE_ADDRESS_MODE_CLAMP,
                AddressW = D3D12_TEXTURE_ADDRESS_MODE_CLAMP,
                BorderColor = D3D12_STATIC_BORDER_COLOR_OPAQUE_BLACK,
                ComparisonFunc = D3D12_COMPARISON_FUNC_NEVER,
                Filter = D3D12_FILTER_COMPARISON_MIN_MAG_MIP_LINEAR,
                MaxAnisotropy = 1,
                MaxLOD = D3D12_FLOAT32_MAX,
                MinLOD = 0,
                MipLODBias = 0,
                ShaderVisibility = visibiliy,
                RegisterSpace = registerSpace,
                ShaderRegister = register
            },
            SampleState.Point => new D3D12_STATIC_SAMPLER_DESC
            {
                AddressU = D3D12_TEXTURE_ADDRESS_MODE_CLAMP,
                AddressV = D3D12_TEXTURE_ADDRESS_MODE_CLAMP,
                AddressW = D3D12_TEXTURE_ADDRESS_MODE_CLAMP,
                BorderColor = D3D12_STATIC_BORDER_COLOR_OPAQUE_BLACK,
                ComparisonFunc = D3D12_COMPARISON_FUNC_NEVER,
                Filter = D3D12_FILTER_COMPARISON_MIN_MAG_MIP_POINT,
                MaxAnisotropy = 1,
                MaxLOD = D3D12_FLOAT32_MAX,
                MinLOD = 0,
                MipLODBias = 0,
                ShaderVisibility = visibiliy,
                RegisterSpace = registerSpace,
                ShaderRegister = register
            },
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };



    public static unsafe void InitSquareIndexBuffer(ushort* buffer, uint size) => InitSquareIndexBuffer(new Span<ushort>(buffer, (int)size));
    public static void InitSquareIndexBuffer(Span<ushort> indices)
    {
        Debug.Assert(indices.Length == 6, "Index buffer only supports a size of 6 elements.");
        indices[0] = 0;
        indices[1] = 1;
        indices[2] = 2;
        indices[3] = 3;
        indices[4] = 0;
        indices[5] = 2;
    }


    public static unsafe void SetName(ID3D12Resource* resource, ReadOnlySpan<char> name)
    {
        //NOTE(Jens): can we use a common interface for this maybe? or just create overloads?
        fixed (char* namePtr = name)
        {
            var hr = resource->SetName(namePtr);
            Debug.Assert(Win32Common.SUCCEEDED(hr), $"Failed to set the name for resource {name} with HRESULT {hr}");
        }
    }
}
