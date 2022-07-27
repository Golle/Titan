using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Windows;
using Titan.Windows.D3D;
using Titan.Windows.D3D11;
using Titan.Windows.DXGI;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.D3D12;

public static unsafe class D3D12Common
{

    private const string DllName = "d3d12";
    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT D3D12CreateDevice(
        IUnknown* pAdapter,
        D3D_FEATURE_LEVEL minimumFeatureLevel,
        in Guid riid, // Expected: ID3D12Device
        void** ppDevice);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT D3D12GetDebugInterface(
        in Guid riid,
        void** ppvDebug
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT D3D12SerializeRootSignature(
        D3D12_ROOT_SIGNATURE_DESC* pRootSignature,
        D3D_ROOT_SIGNATURE_VERSION Version,
        ID3DBlob** ppBlob,
        ID3DBlob** ppErrorBlob
    );
}

public unsafe struct D3D12_ROOT_SIGNATURE_DESC
{
    public uint NumParameters;
    public D3D12_ROOT_PARAMETER* pParameters;
    public uint NumStaticSamplers;
    public D3D12_STATIC_SAMPLER_DESC* pStaticSamplers;
    public D3D12_ROOT_SIGNATURE_FLAGS Flags;
}

public struct D3D12_ROOT_PARAMETER
{
    public D3D12_ROOT_PARAMETER_TYPE ParameterType;
    private D3D12_ROOT_PARAMETER_UNION UnionMembers;
    public unsafe ref D3D12_ROOT_DESCRIPTOR_TABLE DescriptorTable => ref ((D3D12_ROOT_PARAMETER_UNION*)Unsafe.AsPointer(ref UnionMembers))->DescriptorTable;
    public unsafe ref D3D12_ROOT_CONSTANTS Constants => ref ((D3D12_ROOT_PARAMETER_UNION*)Unsafe.AsPointer(ref UnionMembers))->Constants;
    public unsafe ref D3D12_ROOT_DESCRIPTOR Descriptor => ref ((D3D12_ROOT_PARAMETER_UNION*)Unsafe.AsPointer(ref UnionMembers))->Descriptor;
    public D3D12_SHADER_VISIBILITY ShaderVisibility;
    [StructLayout(LayoutKind.Explicit)]
    private struct D3D12_ROOT_PARAMETER_UNION
    {
        [FieldOffset(0)]
        public D3D12_ROOT_DESCRIPTOR_TABLE DescriptorTable;
        [FieldOffset(0)]
        public D3D12_ROOT_CONSTANTS Constants;
        [FieldOffset(0)]
        public D3D12_ROOT_DESCRIPTOR Descriptor;
    }
}

public struct D3D12_ROOT_DESCRIPTOR
{
    public uint ShaderRegister;
    public uint RegisterSpace;
}

public unsafe struct D3D12_ROOT_DESCRIPTOR_TABLE
{
    public uint NumDescriptorRanges;
    public D3D12_DESCRIPTOR_RANGE* pDescriptorRanges;
}

public struct D3D12_DESCRIPTOR_RANGE
{
    public D3D12_DESCRIPTOR_RANGE_TYPE RangeType;
    public uint NumDescriptors;
    public uint BaseShaderRegister;
    public uint RegisterSpace;
    public uint OffsetInDescriptorsFromTableStart;
}

public unsafe struct D3D12_ROOT_DESCRIPTOR_TABLE1
{
    public uint NumDescriptorRanges;
    public D3D12_DESCRIPTOR_RANGE1* pDescriptorRanges;
}

public struct D3D12_DESCRIPTOR_RANGE1
{
    public D3D12_DESCRIPTOR_RANGE_TYPE RangeType;
    public uint NumDescriptors;
    public uint BaseShaderRegister;
    public uint RegisterSpace;
    public D3D12_DESCRIPTOR_RANGE_FLAGS Flags;
    public uint OffsetInDescriptorsFromTableStart;
}
public enum D3D12_DESCRIPTOR_RANGE_FLAGS
{
    D3D12_DESCRIPTOR_RANGE_FLAG_NONE = 0,
    D3D12_DESCRIPTOR_RANGE_FLAG_DESCRIPTORS_VOLATILE = 0x1,
    D3D12_DESCRIPTOR_RANGE_FLAG_DATA_VOLATILE = 0x2,
    D3D12_DESCRIPTOR_RANGE_FLAG_DATA_STATIC_WHILE_SET_AT_EXECUTE = 0x4,
    D3D12_DESCRIPTOR_RANGE_FLAG_DATA_STATIC = 0x8,
    D3D12_DESCRIPTOR_RANGE_FLAG_DESCRIPTORS_STATIC_KEEPING_BUFFER_BOUNDS_CHECKS = 0x10000
}
public enum D3D12_DESCRIPTOR_RANGE_TYPE
{
    D3D12_DESCRIPTOR_RANGE_TYPE_SRV = 0,
    D3D12_DESCRIPTOR_RANGE_TYPE_UAV = (D3D12_DESCRIPTOR_RANGE_TYPE_SRV + 1),
    D3D12_DESCRIPTOR_RANGE_TYPE_CBV = (D3D12_DESCRIPTOR_RANGE_TYPE_UAV + 1),
    D3D12_DESCRIPTOR_RANGE_TYPE_SAMPLER = (D3D12_DESCRIPTOR_RANGE_TYPE_CBV + 1)
}
public struct D3D12_ROOT_PARAMETER1
{
    public D3D12_ROOT_PARAMETER_TYPE ParameterType;
    private D3D12_ROOT_PARAMETER1_UNION UnionMembers;
    public unsafe ref D3D12_ROOT_DESCRIPTOR_TABLE1 DescriptorTable => ref ((D3D12_ROOT_PARAMETER1_UNION*)Unsafe.AsPointer(ref UnionMembers))->DescriptorTable;
    public unsafe ref D3D12_ROOT_CONSTANTS Constants => ref ((D3D12_ROOT_PARAMETER1_UNION*)Unsafe.AsPointer(ref UnionMembers))->Constants;
    public unsafe ref D3D12_ROOT_DESCRIPTOR1 Descriptor => ref ((D3D12_ROOT_PARAMETER1_UNION*)Unsafe.AsPointer(ref UnionMembers))->Descriptor;
    public D3D12_SHADER_VISIBILITY ShaderVisibility;
    [StructLayout(LayoutKind.Explicit)]
    private struct D3D12_ROOT_PARAMETER1_UNION
    {
        [FieldOffset(0)]
        public D3D12_ROOT_DESCRIPTOR_TABLE1 DescriptorTable;
        [FieldOffset(0)]
        public D3D12_ROOT_CONSTANTS Constants;
        [FieldOffset(0)]
        public D3D12_ROOT_DESCRIPTOR1 Descriptor;
    }
}

public struct D3D12_ROOT_CONSTANTS
{
    public uint ShaderRegister;
    public uint RegisterSpace;
    public uint Num32BitValues;
}
public struct D3D12_ROOT_DESCRIPTOR1
{
    public uint ShaderRegister;
    public uint RegisterSpace;
    D3D12_ROOT_DESCRIPTOR_FLAGS Flags;
}

[Flags]
public enum D3D12_ROOT_DESCRIPTOR_FLAGS
{
    D3D12_ROOT_DESCRIPTOR_FLAG_NONE = 0,
    D3D12_ROOT_DESCRIPTOR_FLAG_DATA_VOLATILE = 0x2,
    D3D12_ROOT_DESCRIPTOR_FLAG_DATA_STATIC_WHILE_SET_AT_EXECUTE = 0x4,
    D3D12_ROOT_DESCRIPTOR_FLAG_DATA_STATIC = 0x8
}
public struct D3D12_STATIC_SAMPLER_DESC
{
    public D3D12_FILTER Filter;
    public D3D12_TEXTURE_ADDRESS_MODE AddressU;
    public D3D12_TEXTURE_ADDRESS_MODE AddressV;
    public D3D12_TEXTURE_ADDRESS_MODE AddressW;
    public float MipLODBias;
    public uint MaxAnisotropy;
    public D3D12_COMPARISON_FUNC ComparisonFunc;
    public D3D12_STATIC_BORDER_COLOR BorderColor;
    public float MinLOD;
    public float MaxLOD;
    public uint ShaderRegister;
    public uint RegisterSpace;
    public D3D12_SHADER_VISIBILITY ShaderVisibility;
}

public enum D3D12_STATIC_BORDER_COLOR
{
    D3D12_STATIC_BORDER_COLOR_TRANSPARENT_BLACK = 0,
    D3D12_STATIC_BORDER_COLOR_OPAQUE_BLACK = (D3D12_STATIC_BORDER_COLOR_TRANSPARENT_BLACK + 1),
    D3D12_STATIC_BORDER_COLOR_OPAQUE_WHITE = (D3D12_STATIC_BORDER_COLOR_OPAQUE_BLACK + 1)
}
public enum D3D12_FILTER
{
    D3D12_FILTER_MIN_MAG_MIP_POINT = 0,
    D3D12_FILTER_MIN_MAG_POINT_MIP_LINEAR = 0x1,
    D3D12_FILTER_MIN_POINT_MAG_LINEAR_MIP_POINT = 0x4,
    D3D12_FILTER_MIN_POINT_MAG_MIP_LINEAR = 0x5,
    D3D12_FILTER_MIN_LINEAR_MAG_MIP_POINT = 0x10,
    D3D12_FILTER_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x11,
    D3D12_FILTER_MIN_MAG_LINEAR_MIP_POINT = 0x14,
    D3D12_FILTER_MIN_MAG_MIP_LINEAR = 0x15,
    D3D12_FILTER_ANISOTROPIC = 0x55,
    D3D12_FILTER_COMPARISON_MIN_MAG_MIP_POINT = 0x80,
    D3D12_FILTER_COMPARISON_MIN_MAG_POINT_MIP_LINEAR = 0x81,
    D3D12_FILTER_COMPARISON_MIN_POINT_MAG_LINEAR_MIP_POINT = 0x84,
    D3D12_FILTER_COMPARISON_MIN_POINT_MAG_MIP_LINEAR = 0x85,
    D3D12_FILTER_COMPARISON_MIN_LINEAR_MAG_MIP_POINT = 0x90,
    D3D12_FILTER_COMPARISON_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x91,
    D3D12_FILTER_COMPARISON_MIN_MAG_LINEAR_MIP_POINT = 0x94,
    D3D12_FILTER_COMPARISON_MIN_MAG_MIP_LINEAR = 0x95,
    D3D12_FILTER_COMPARISON_ANISOTROPIC = 0xd5,
    D3D12_FILTER_MINIMUM_MIN_MAG_MIP_POINT = 0x100,
    D3D12_FILTER_MINIMUM_MIN_MAG_POINT_MIP_LINEAR = 0x101,
    D3D12_FILTER_MINIMUM_MIN_POINT_MAG_LINEAR_MIP_POINT = 0x104,
    D3D12_FILTER_MINIMUM_MIN_POINT_MAG_MIP_LINEAR = 0x105,
    D3D12_FILTER_MINIMUM_MIN_LINEAR_MAG_MIP_POINT = 0x110,
    D3D12_FILTER_MINIMUM_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x111,
    D3D12_FILTER_MINIMUM_MIN_MAG_LINEAR_MIP_POINT = 0x114,
    D3D12_FILTER_MINIMUM_MIN_MAG_MIP_LINEAR = 0x115,
    D3D12_FILTER_MINIMUM_ANISOTROPIC = 0x155,
    D3D12_FILTER_MAXIMUM_MIN_MAG_MIP_POINT = 0x180,
    D3D12_FILTER_MAXIMUM_MIN_MAG_POINT_MIP_LINEAR = 0x181,
    D3D12_FILTER_MAXIMUM_MIN_POINT_MAG_LINEAR_MIP_POINT = 0x184,
    D3D12_FILTER_MAXIMUM_MIN_POINT_MAG_MIP_LINEAR = 0x185,
    D3D12_FILTER_MAXIMUM_MIN_LINEAR_MAG_MIP_POINT = 0x190,
    D3D12_FILTER_MAXIMUM_MIN_LINEAR_MAG_POINT_MIP_LINEAR = 0x191,
    D3D12_FILTER_MAXIMUM_MIN_MAG_LINEAR_MIP_POINT = 0x194,
    D3D12_FILTER_MAXIMUM_MIN_MAG_MIP_LINEAR = 0x195,
    D3D12_FILTER_MAXIMUM_ANISOTROPIC = 0x1d5
}
public enum D3D12_COMPARISON_FUNC
{
    D3D12_COMPARISON_FUNC_NEVER = 1,
    D3D12_COMPARISON_FUNC_LESS = 2,
    D3D12_COMPARISON_FUNC_EQUAL = 3,
    D3D12_COMPARISON_FUNC_LESS_EQUAL = 4,
    D3D12_COMPARISON_FUNC_GREATER = 5,
    D3D12_COMPARISON_FUNC_NOT_EQUAL = 6,
    D3D12_COMPARISON_FUNC_GREATER_EQUAL = 7,
    D3D12_COMPARISON_FUNC_ALWAYS = 8
}
public enum D3D12_TEXTURE_ADDRESS_MODE
{
    D3D12_TEXTURE_ADDRESS_MODE_WRAP = 1,
    D3D12_TEXTURE_ADDRESS_MODE_MIRROR = 2,
    D3D12_TEXTURE_ADDRESS_MODE_CLAMP = 3,
    D3D12_TEXTURE_ADDRESS_MODE_BORDER = 4,
    D3D12_TEXTURE_ADDRESS_MODE_MIRROR_ONCE = 5
}

public enum D3D12_SHADER_VISIBILITY
{
    D3D12_SHADER_VISIBILITY_ALL = 0,
    D3D12_SHADER_VISIBILITY_VERTEX = 1,
    D3D12_SHADER_VISIBILITY_HULL = 2,
    D3D12_SHADER_VISIBILITY_DOMAIN = 3,
    D3D12_SHADER_VISIBILITY_GEOMETRY = 4,
    D3D12_SHADER_VISIBILITY_PIXEL = 5,
    D3D12_SHADER_VISIBILITY_AMPLIFICATION = 6,
    D3D12_SHADER_VISIBILITY_MESH = 7
}

public enum D3D12_ROOT_PARAMETER_TYPE
{
    D3D12_ROOT_PARAMETER_TYPE_DESCRIPTOR_TABLE = 0,
    D3D12_ROOT_PARAMETER_TYPE_32BIT_CONSTANTS = (D3D12_ROOT_PARAMETER_TYPE_DESCRIPTOR_TABLE + 1),
    D3D12_ROOT_PARAMETER_TYPE_CBV = (D3D12_ROOT_PARAMETER_TYPE_32BIT_CONSTANTS + 1),
    D3D12_ROOT_PARAMETER_TYPE_SRV = (D3D12_ROOT_PARAMETER_TYPE_CBV + 1),
    D3D12_ROOT_PARAMETER_TYPE_UAV = (D3D12_ROOT_PARAMETER_TYPE_SRV + 1)
}

[Flags]
public enum D3D12_ROOT_SIGNATURE_FLAGS
{
    D3D12_ROOT_SIGNATURE_FLAG_NONE = 0,
    D3D12_ROOT_SIGNATURE_FLAG_ALLOW_INPUT_ASSEMBLER_INPUT_LAYOUT = 0x1,
    D3D12_ROOT_SIGNATURE_FLAG_DENY_VERTEX_SHADER_ROOT_ACCESS = 0x2,
    D3D12_ROOT_SIGNATURE_FLAG_DENY_HULL_SHADER_ROOT_ACCESS = 0x4,
    D3D12_ROOT_SIGNATURE_FLAG_DENY_DOMAIN_SHADER_ROOT_ACCESS = 0x8,
    D3D12_ROOT_SIGNATURE_FLAG_DENY_GEOMETRY_SHADER_ROOT_ACCESS = 0x10,
    D3D12_ROOT_SIGNATURE_FLAG_DENY_PIXEL_SHADER_ROOT_ACCESS = 0x20,
    D3D12_ROOT_SIGNATURE_FLAG_ALLOW_STREAM_OUTPUT = 0x40,
    D3D12_ROOT_SIGNATURE_FLAG_LOCAL_ROOT_SIGNATURE = 0x80,
    D3D12_ROOT_SIGNATURE_FLAG_DENY_AMPLIFICATION_SHADER_ROOT_ACCESS = 0x100,
    D3D12_ROOT_SIGNATURE_FLAG_DENY_MESH_SHADER_ROOT_ACCESS = 0x200,
    D3D12_ROOT_SIGNATURE_FLAG_CBV_SRV_UAV_HEAP_DIRECTLY_INDEXED = 0x400,
    D3D12_ROOT_SIGNATURE_FLAG_SAMPLER_HEAP_DIRECTLY_INDEXED = 0x800
}

public unsafe struct D3D12_SHADER_BYTECODE
{
    public void* pShaderBytecode;
    public nuint BytecodeLength;
}

public unsafe struct D3D12_STREAM_OUTPUT_DESC
{
    public D3D12_SO_DECLARATION_ENTRY* pSODeclaration;
    public uint NumEntries;
    public uint* pBufferStrides;
    public uint NumStrides;
    public uint RasterizedStream;
}

public unsafe struct D3D12_SO_DECLARATION_ENTRY
{
    public uint Stream;
    public byte* SemanticName;
    public uint SemanticIndex;
    public byte StartComponent;
    public byte ComponentCount;
    public byte OutputSlot;
}

public unsafe struct D3D12_GRAPHICS_PIPELINE_STATE_DESC
{
    public ID3D12RootSignature* pRootSignature;
    public D3D12_SHADER_BYTECODE VS;
    public D3D12_SHADER_BYTECODE PS;
    public D3D12_SHADER_BYTECODE DS;
    public D3D12_SHADER_BYTECODE HS;
    public D3D12_SHADER_BYTECODE GS;
    public D3D12_STREAM_OUTPUT_DESC StreamOutput;
    public D3D12_BLEND_DESC BlendState;
    public uint SampleMask;
    public D3D12_RASTERIZER_DESC RasterizerState;
    public D3D12_DEPTH_STENCIL_DESC DepthStencilState;
    public D3D12_INPUT_LAYOUT_DESC InputLayout;
    public D3D12_INDEX_BUFFER_STRIP_CUT_VALUE IBStripCutValue;
    public D3D12_PRIMITIVE_TOPOLOGY_TYPE PrimitiveTopologyType;
    public uint NumRenderTargets;
    //public DXGI_FORMAT RTVFormats[8];
    private D3D12_GRAPHICS_PIPELINE_STATE_DESC_RTV_FORMATS _rtvFormats;
    public DXGI_FORMAT* RTVFormats => (DXGI_FORMAT*)Unsafe.AsPointer(ref _rtvFormats);
    public DXGI_FORMAT DSVFormat;
    public DXGI_SAMPLE_DESC SampleDesc;
    public uint NodeMask;
    public D3D12_CACHED_PIPELINE_STATE CachedPSO;
    public D3D12_PIPELINE_STATE_FLAGS Flags;

    private struct D3D12_GRAPHICS_PIPELINE_STATE_DESC_RTV_FORMATS
    {
        public DXGI_FORMAT RTVFormats1, RTVFormats2, RTVFormats3, RTVFormats4, RTVFormats5, RTVFormats6, RTVFormats7, RTVFormats8;
    }
}
public enum D3D12_PIPELINE_STATE_FLAGS
{
    D3D12_PIPELINE_STATE_FLAG_NONE = 0,
    D3D12_PIPELINE_STATE_FLAG_TOOL_DEBUG = 0x1
}
public unsafe struct D3D12_CACHED_PIPELINE_STATE
{
    public void* pCachedBlob;
    public nuint CachedBlobSizeInBytes;
}
public enum D3D12_PRIMITIVE_TOPOLOGY_TYPE
{
    D3D12_PRIMITIVE_TOPOLOGY_TYPE_UNDEFINED = 0,
    D3D12_PRIMITIVE_TOPOLOGY_TYPE_POINT = 1,
    D3D12_PRIMITIVE_TOPOLOGY_TYPE_LINE = 2,
    D3D12_PRIMITIVE_TOPOLOGY_TYPE_TRIANGLE = 3,
    D3D12_PRIMITIVE_TOPOLOGY_TYPE_PATCH = 4
}
public unsafe struct D3D12_INPUT_LAYOUT_DESC
{
    public D3D12_INPUT_ELEMENT_DESC* pInputElementDescs;
    public uint NumElements;
}
public enum D3D12_INDEX_BUFFER_STRIP_CUT_VALUE
{
    D3D12_INDEX_BUFFER_STRIP_CUT_VALUE_DISABLED = 0,
    D3D12_INDEX_BUFFER_STRIP_CUT_VALUE_0xFFFF = 1,
    D3D12_INDEX_BUFFER_STRIP_CUT_VALUE_0xFFFFFFFF = 2
}
public struct D3D12_DEPTH_STENCIL_DESC
{
    public int DepthEnable; //unmanaged bool
    public D3D12_DEPTH_WRITE_MASK DepthWriteMask;
    public D3D12_COMPARISON_FUNC DepthFunc;
    public int StencilEnable;//unmanaged bool
    public byte StencilReadMask;
    public byte StencilWriteMask;
    public D3D12_DEPTH_STENCILOP_DESC FrontFace;
    public D3D12_DEPTH_STENCILOP_DESC BackFace;
}

public struct D3D12_DEPTH_STENCILOP_DESC
{
    public D3D12_STENCIL_OP StencilFailOp;
    public D3D12_STENCIL_OP StencilDepthFailOp;
    public D3D12_STENCIL_OP StencilPassOp;
    public D3D12_COMPARISON_FUNC StencilFunc;
}

public enum D3D12_STENCIL_OP
{
    D3D12_STENCIL_OP_KEEP = 1,
    D3D12_STENCIL_OP_ZERO = 2,
    D3D12_STENCIL_OP_REPLACE = 3,
    D3D12_STENCIL_OP_INCR_SAT = 4,
    D3D12_STENCIL_OP_DECR_SAT = 5,
    D3D12_STENCIL_OP_INVERT = 6,
    D3D12_STENCIL_OP_INCR = 7,
    D3D12_STENCIL_OP_DECR = 8
}

public enum D3D12_DEPTH_WRITE_MASK
{
    D3D12_DEPTH_WRITE_MASK_ZERO = 0,
    D3D12_DEPTH_WRITE_MASK_ALL = 1
}
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
public enum D3D12_CONSERVATIVE_RASTERIZATION_MODE
{
    D3D12_CONSERVATIVE_RASTERIZATION_MODE_OFF = 0,
    D3D12_CONSERVATIVE_RASTERIZATION_MODE_ON = 1
}
public enum D3D12_FILL_MODE
{
    D3D12_FILL_MODE_WIREFRAME = 2,
    D3D12_FILL_MODE_SOLID = 3
}

public enum D3D12_CULL_MODE
{
    D3D12_CULL_MODE_NONE = 1,
    D3D12_CULL_MODE_FRONT = 2,
    D3D12_CULL_MODE_BACK = 3
}
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


[Flags]
public enum D3D12_COLOR_WRITE_ENABLE : byte
{
    D3D12_COLOR_WRITE_ENABLE_RED = 1,
    D3D12_COLOR_WRITE_ENABLE_GREEN = 2,
    D3D12_COLOR_WRITE_ENABLE_BLUE = 4,
    D3D12_COLOR_WRITE_ENABLE_ALPHA = 8,
    D3D12_COLOR_WRITE_ENABLE_ALL = (((D3D12_COLOR_WRITE_ENABLE_RED | D3D12_COLOR_WRITE_ENABLE_GREEN) | D3D12_COLOR_WRITE_ENABLE_BLUE) | D3D12_COLOR_WRITE_ENABLE_ALPHA)
}

public struct D3D12_RENDER_TARGET_BLEND_DESC
{
    public int BlendEnable;// Unmanaged BOOL
    public int LogicOpEnable;// Unmanaged BOOL
    public D3D12_BLEND SrcBlend;
    public D3D12_BLEND DestBlend;
    public D3D12_BLEND_OP BlendOp;
    public D3D12_BLEND SrcBlendAlpha;
    public D3D12_BLEND DestBlendAlpha;
    public D3D12_BLEND_OP BlendOpAlpha;
    public D3D12_LOGIC_OP LogicOp;
    public D3D12_COLOR_WRITE_ENABLE RenderTargetWriteMask;
}

public enum D3D12_LOGIC_OP
{
    D3D12_LOGIC_OP_CLEAR = 0,
    D3D12_LOGIC_OP_SET = (D3D12_LOGIC_OP_CLEAR + 1),
    D3D12_LOGIC_OP_COPY = (D3D12_LOGIC_OP_SET + 1),
    D3D12_LOGIC_OP_COPY_INVERTED = (D3D12_LOGIC_OP_COPY + 1),
    D3D12_LOGIC_OP_NOOP = (D3D12_LOGIC_OP_COPY_INVERTED + 1),
    D3D12_LOGIC_OP_INVERT = (D3D12_LOGIC_OP_NOOP + 1),
    D3D12_LOGIC_OP_AND = (D3D12_LOGIC_OP_INVERT + 1),
    D3D12_LOGIC_OP_NAND = (D3D12_LOGIC_OP_AND + 1),
    D3D12_LOGIC_OP_OR = (D3D12_LOGIC_OP_NAND + 1),
    D3D12_LOGIC_OP_NOR = (D3D12_LOGIC_OP_OR + 1),
    D3D12_LOGIC_OP_XOR = (D3D12_LOGIC_OP_NOR + 1),
    D3D12_LOGIC_OP_EQUIV = (D3D12_LOGIC_OP_XOR + 1),
    D3D12_LOGIC_OP_AND_REVERSE = (D3D12_LOGIC_OP_EQUIV + 1),
    D3D12_LOGIC_OP_AND_INVERTED = (D3D12_LOGIC_OP_AND_REVERSE + 1),
    D3D12_LOGIC_OP_OR_REVERSE = (D3D12_LOGIC_OP_AND_INVERTED + 1),
    D3D12_LOGIC_OP_OR_INVERTED = (D3D12_LOGIC_OP_OR_REVERSE + 1)
}
public enum D3D12_BLEND_OP
{
    D3D12_BLEND_OP_ADD = 1,
    D3D12_BLEND_OP_SUBTRACT = 2,
    D3D12_BLEND_OP_REV_SUBTRACT = 3,
    D3D12_BLEND_OP_MIN = 4,
    D3D12_BLEND_OP_MAX = 5
}
public enum D3D12_BLEND
{
    D3D12_BLEND_ZERO = 1,
    D3D12_BLEND_ONE = 2,
    D3D12_BLEND_SRC_COLOR = 3,
    D3D12_BLEND_INV_SRC_COLOR = 4,
    D3D12_BLEND_SRC_ALPHA = 5,
    D3D12_BLEND_INV_SRC_ALPHA = 6,
    D3D12_BLEND_DEST_ALPHA = 7,
    D3D12_BLEND_INV_DEST_ALPHA = 8,
    D3D12_BLEND_DEST_COLOR = 9,
    D3D12_BLEND_INV_DEST_COLOR = 10,
    D3D12_BLEND_SRC_ALPHA_SAT = 11,
    D3D12_BLEND_BLEND_FACTOR = 14,
    D3D12_BLEND_INV_BLEND_FACTOR = 15,
    D3D12_BLEND_SRC1_COLOR = 16,
    D3D12_BLEND_INV_SRC1_COLOR = 17,
    D3D12_BLEND_SRC1_ALPHA = 18,
    D3D12_BLEND_INV_SRC1_ALPHA = 19
}


public struct D3D12_HEAP_PROPERTIES
{
    public D3D12_HEAP_TYPE Type;
    public D3D12_CPU_PAGE_PROPERTY CPUPageProperty;
    public D3D12_MEMORY_POOL MemoryPoolPreference;
    public uint CreationNodeMask;
    public uint VisibleNodeMask;
}
public enum D3D12_MEMORY_POOL
{
    D3D12_MEMORY_POOL_UNKNOWN = 0,
    D3D12_MEMORY_POOL_L0 = 1,
    D3D12_MEMORY_POOL_L1 = 2
}
public enum D3D12_CPU_PAGE_PROPERTY
{
    D3D12_CPU_PAGE_PROPERTY_UNKNOWN = 0,
    D3D12_CPU_PAGE_PROPERTY_NOT_AVAILABLE = 1,
    D3D12_CPU_PAGE_PROPERTY_WRITE_COMBINE = 2,
    D3D12_CPU_PAGE_PROPERTY_WRITE_BACK = 3
}
public enum D3D12_HEAP_TYPE
{
    D3D12_HEAP_TYPE_DEFAULT = 1,
    D3D12_HEAP_TYPE_UPLOAD = 2,
    D3D12_HEAP_TYPE_READBACK = 3,
    D3D12_HEAP_TYPE_CUSTOM = 4
}
[Flags]
public enum D3D12_HEAP_FLAGS
{
    D3D12_HEAP_FLAG_NONE = 0,
    D3D12_HEAP_FLAG_SHARED = 0x1,
    D3D12_HEAP_FLAG_DENY_BUFFERS = 0x4,
    D3D12_HEAP_FLAG_ALLOW_DISPLAY = 0x8,
    D3D12_HEAP_FLAG_SHARED_CROSS_ADAPTER = 0x20,
    D3D12_HEAP_FLAG_DENY_RT_DS_TEXTURES = 0x40,
    D3D12_HEAP_FLAG_DENY_NON_RT_DS_TEXTURES = 0x80,
    D3D12_HEAP_FLAG_HARDWARE_PROTECTED = 0x100,
    D3D12_HEAP_FLAG_ALLOW_WRITE_WATCH = 0x200,
    D3D12_HEAP_FLAG_ALLOW_SHADER_ATOMICS = 0x400,
    D3D12_HEAP_FLAG_CREATE_NOT_RESIDENT = 0x800,
    D3D12_HEAP_FLAG_CREATE_NOT_ZEROED = 0x1000,
    D3D12_HEAP_FLAG_ALLOW_ALL_BUFFERS_AND_TEXTURES = 0,
    D3D12_HEAP_FLAG_ALLOW_ONLY_BUFFERS = 0xc0,
    D3D12_HEAP_FLAG_ALLOW_ONLY_NON_RT_DS_TEXTURES = 0x44,
    D3D12_HEAP_FLAG_ALLOW_ONLY_RT_DS_TEXTURES = 0x84
}

public struct D3D12_RESOURCE_DESC
{
    public D3D12_RESOURCE_DIMENSION Dimension;
    public ulong Alignment;
    public ulong Width;
    public uint Height;
    public ushort DepthOrArraySize;
    public ushort MipLevels;
    public DXGI_FORMAT Format;
    public DXGI_SAMPLE_DESC SampleDesc;
    public D3D12_TEXTURE_LAYOUT Layout;
    public D3D12_RESOURCE_FLAGS Flags;
}

public enum D3D12_RESOURCE_DIMENSION
{
    D3D12_RESOURCE_DIMENSION_UNKNOWN = 0,
    D3D12_RESOURCE_DIMENSION_BUFFER = 1,
    D3D12_RESOURCE_DIMENSION_TEXTURE1D = 2,
    D3D12_RESOURCE_DIMENSION_TEXTURE2D = 3,
    D3D12_RESOURCE_DIMENSION_TEXTURE3D = 4
}

public enum D3D12_TEXTURE_LAYOUT
{
    D3D12_TEXTURE_LAYOUT_UNKNOWN = 0,
    D3D12_TEXTURE_LAYOUT_ROW_MAJOR = 1,
    D3D12_TEXTURE_LAYOUT_64KB_UNDEFINED_SWIZZLE = 2,
    D3D12_TEXTURE_LAYOUT_64KB_STANDARD_SWIZZLE = 3
}
[Flags]
public enum D3D12_RESOURCE_FLAGS
{
    D3D12_RESOURCE_FLAG_NONE = 0,
    D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET = 0x1,
    D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL = 0x2,
    D3D12_RESOURCE_FLAG_ALLOW_UNORDERED_ACCESS = 0x4,
    D3D12_RESOURCE_FLAG_DENY_SHADER_RESOURCE = 0x8,
    D3D12_RESOURCE_FLAG_ALLOW_CROSS_ADAPTER = 0x10,
    D3D12_RESOURCE_FLAG_ALLOW_SIMULTANEOUS_ACCESS = 0x20,
    D3D12_RESOURCE_FLAG_VIDEO_DECODE_REFERENCE_ONLY = 0x40,
    D3D12_RESOURCE_FLAG_VIDEO_ENCODE_REFERENCE_ONLY = 0x80
}

[Flags]
public enum D3D12_RESOURCE_STATES
{
    D3D12_RESOURCE_STATE_COMMON = 0,
    D3D12_RESOURCE_STATE_VERTEX_AND_CONSTANT_BUFFER = 0x1,
    D3D12_RESOURCE_STATE_INDEX_BUFFER = 0x2,
    D3D12_RESOURCE_STATE_RENDER_TARGET = 0x4,
    D3D12_RESOURCE_STATE_UNORDERED_ACCESS = 0x8,
    D3D12_RESOURCE_STATE_DEPTH_WRITE = 0x10,
    D3D12_RESOURCE_STATE_DEPTH_READ = 0x20,
    D3D12_RESOURCE_STATE_NON_PIXEL_SHADER_RESOURCE = 0x40,
    D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE = 0x80,
    D3D12_RESOURCE_STATE_STREAM_OUT = 0x100,
    D3D12_RESOURCE_STATE_INDIRECT_ARGUMENT = 0x200,
    D3D12_RESOURCE_STATE_COPY_DEST = 0x400,
    D3D12_RESOURCE_STATE_COPY_SOURCE = 0x800,
    D3D12_RESOURCE_STATE_RESOLVE_DEST = 0x1000,
    D3D12_RESOURCE_STATE_RESOLVE_SOURCE = 0x2000,
    D3D12_RESOURCE_STATE_RAYTRACING_ACCELERATION_STRUCTURE = 0x400000,
    D3D12_RESOURCE_STATE_SHADING_RATE_SOURCE = 0x1000000,
    D3D12_RESOURCE_STATE_GENERIC_READ = (((((0x1 | 0x2) | 0x40) | 0x80) | 0x200) | 0x800),
    D3D12_RESOURCE_STATE_ALL_SHADER_RESOURCE = (0x40 | 0x80),
    D3D12_RESOURCE_STATE_PRESENT = 0,
    D3D12_RESOURCE_STATE_PREDICATION = 0x200,
    D3D12_RESOURCE_STATE_VIDEO_DECODE_READ = 0x10000,
    D3D12_RESOURCE_STATE_VIDEO_DECODE_WRITE = 0x20000,
    D3D12_RESOURCE_STATE_VIDEO_PROCESS_READ = 0x40000,
    D3D12_RESOURCE_STATE_VIDEO_PROCESS_WRITE = 0x80000,
    D3D12_RESOURCE_STATE_VIDEO_ENCODE_READ = 0x200000,
    D3D12_RESOURCE_STATE_VIDEO_ENCODE_WRITE = 0x800000
}

public unsafe struct D3D12_CLEAR_VALUE
{
    public DXGI_FORMAT Format;

    private D3D12_CLEAR_VALUE_UNION _union;
    public float* Color => ((D3D12_CLEAR_VALUE_UNION*)Unsafe.AsPointer(ref _union))->Color;
    public ref D3D12_DEPTH_STENCIL_VALUE DepthStencil => ref ((D3D12_CLEAR_VALUE_UNION*)Unsafe.AsPointer(ref _union))->DepthStencil;

    [StructLayout(LayoutKind.Explicit)]
    private struct D3D12_CLEAR_VALUE_UNION
    {
        [FieldOffset(0)]
        public fixed float Color[4];
        [FieldOffset(0)]
        public D3D12_DEPTH_STENCIL_VALUE DepthStencil;
    }
}

public struct D3D12_DEPTH_STENCIL_VALUE
{
    public float Depth;
    public byte Stencil;
}


public struct D3D12_VERTEX_BUFFER_VIEW
{
    public D3D12_GPU_VIRTUAL_ADDRESS BufferLocation;
    public uint SizeInBytes;
    public uint StrideInBytes;
}


public struct D3D12_GPU_VIRTUAL_ADDRESS
{
    public ulong Address;
    public D3D12_GPU_VIRTUAL_ADDRESS(ulong address) => Address = address;
    public static implicit operator ulong(in D3D12_GPU_VIRTUAL_ADDRESS a) => a.Address;
    public static implicit operator D3D12_GPU_VIRTUAL_ADDRESS(ulong address) => new(address);
}


public struct D3D12_RESOURCE_BARRIER
{
    public D3D12_RESOURCE_BARRIER_TYPE Type;
    public D3D12_RESOURCE_BARRIER_FLAGS Flags;
    private D3D12_RESOURCE_BARRIER_UNION _union;
    public unsafe ref D3D12_RESOURCE_TRANSITION_BARRIER Transition => ref ((D3D12_RESOURCE_BARRIER_UNION*)Unsafe.AsPointer(ref _union))->Transition;
    public unsafe ref D3D12_RESOURCE_ALIASING_BARRIER Aliasing => ref ((D3D12_RESOURCE_BARRIER_UNION*)Unsafe.AsPointer(ref _union))->Aliasing;
    public unsafe ref D3D12_RESOURCE_UAV_BARRIER UAV => ref ((D3D12_RESOURCE_BARRIER_UNION*)Unsafe.AsPointer(ref _union))->UAV;
    [StructLayout(LayoutKind.Explicit)]
    private struct D3D12_RESOURCE_BARRIER_UNION
    {
        [FieldOffset(0)]
        public D3D12_RESOURCE_TRANSITION_BARRIER Transition;
        [FieldOffset(0)]
        public D3D12_RESOURCE_ALIASING_BARRIER Aliasing;
        [FieldOffset(0)]
        public D3D12_RESOURCE_UAV_BARRIER UAV;
    }
}

public enum D3D12_RESOURCE_BARRIER_TYPE
{
    D3D12_RESOURCE_BARRIER_TYPE_TRANSITION = 0,
    D3D12_RESOURCE_BARRIER_TYPE_ALIASING = (D3D12_RESOURCE_BARRIER_TYPE_TRANSITION + 1),
    D3D12_RESOURCE_BARRIER_TYPE_UAV = (D3D12_RESOURCE_BARRIER_TYPE_ALIASING + 1)
}

public enum D3D12_RESOURCE_BARRIER_FLAGS
{
    D3D12_RESOURCE_BARRIER_FLAG_NONE = 0,
    D3D12_RESOURCE_BARRIER_FLAG_BEGIN_ONLY = 0x1,
    D3D12_RESOURCE_BARRIER_FLAG_END_ONLY = 0x2
}

public unsafe struct D3D12_RESOURCE_TRANSITION_BARRIER
{
    public ID3D12Resource* pResource;
    public uint Subresource;
    public D3D12_RESOURCE_STATES StateBefore;
    public D3D12_RESOURCE_STATES StateAfter;
}

public unsafe struct D3D12_RESOURCE_ALIASING_BARRIER
{
    public ID3D12Resource* pResourceBefore;
    public ID3D12Resource* pResourceAfter;
}

public unsafe struct D3D12_RESOURCE_UAV_BARRIER
{
    public ID3D12Resource* pResource;
}
