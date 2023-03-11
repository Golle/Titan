namespace Titan.Platform.DXC;

public enum DXC_OUT_KIND : uint
{
    DXC_OUT_NONE = 0,
    DXC_OUT_OBJECT = 1,         // IDxcBlob - Shader or library object
    DXC_OUT_ERRORS = 2,         // IDxcBlobUtf8 or IDxcBlobWide
    DXC_OUT_PDB = 3,            // IDxcBlob
    DXC_OUT_SHADER_HASH = 4,    // IDxcBlob - DxcShaderHash of shader or shader with source info (-Zsb/-Zss)
    DXC_OUT_DISASSEMBLY = 5,    // IDxcBlobUtf8 or IDxcBlobWide - from Disassemble
    DXC_OUT_HLSL = 6,           // IDxcBlobUtf8 or IDxcBlobWide - from Preprocessor or Rewriter
    DXC_OUT_TEXT = 7,           // IDxcBlobUtf8 or IDxcBlobWide - other text, such as -ast-dump or -Odump
    DXC_OUT_REFLECTION = 8,     // IDxcBlob - RDAT part with reflection data
    DXC_OUT_ROOT_SIGNATURE = 9, // IDxcBlob - Serialized root signature output
    DXC_OUT_EXTRA_OUTPUTS = 10,// IDxcExtraResults - Extra outputs
    DXC_OUT_REMARKS = 11,       // IDxcBlobUtf8 or IDxcBlobUtf16 - text directed at stdout

    DXC_OUT_FORCE_DWORD = 0xFFFFFFFF
}
