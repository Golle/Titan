using Titan.Assets;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Platform.Win32.D3D12;

namespace Titan.Graphics.Resources;

public ref struct CreateRootSignatureArgs
{
    public D3D12_ROOT_SIGNATURE_FLAGS Flags;
    public ReadOnlySpan<D3D12_ROOT_PARAMETER1> Parameters;
    public ReadOnlySpan<D3D12_STATIC_SAMPLER_DESC> StaticSamplers;
}

public enum BufferType
{
    Common,
    IndexBuffer,
    ConstantBuffer
}

public record struct CreateBufferArgs(uint Count, uint StrideInBytes, bool CpuVisible = false, bool ShaderVisible = true, BufferType Type = BufferType.Common, TitanBuffer InitialData = default);
public record struct CreateTextureArgs(uint Width, uint Height, TextureFormat Format, bool ShaderVisible = true, bool RenderTargetView = false, TitanBuffer InitialData = default);
public record struct CreateShaderArgs(ShaderType Type, TitanBuffer Data);
public record struct CreateModel3DArgs(uint Vertices, uint Normals, uint UVs, uint IndexCount, int IndexSize, TitanBuffer Data);

public ref struct CreatePipelineStateArgs
{
    public Handle<Shader> VertexShader;
    public Handle<Shader> PixelShader;
    public Handle<Shader> ComputeShader;
    public Handle<RootSignature> RootSignature;
    public ReadOnlySpan<TextureFormat> RenderTargetFormats;
    public PrimitiveTopologyType PrimitiveTopology;
    public BlendStateType BlendState;
}


public unsafe interface IResourceManager
{
    Handle<Texture> CreateTexture(in CreateTextureArgs args);
    bool UploadTexture(in Handle<Texture> handle, TitanBuffer buffer);
    void Destroy(Handle<Texture> handle);
    Texture* AccessTexture(Handle<Texture> handle);

    Handle<Model3D> CreateModel(in CreateModel3DArgs args);
    void DestroyModel(Handle<Model3D> handle);
    Model3D* AccessModel(Handle<Model3D> handle);

    Handle<GPUBuffer> CreateBuffer(in CreateBufferArgs args);
    void DestroyBuffer(Handle<GPUBuffer> handle);
    GPUBuffer* AccessBuffer(Handle<GPUBuffer> handle);


    Handle<Shader> CreateShader(in CreateShaderArgs args);
    void DestroyShader(Handle<Shader> handle);
    Shader* AccessShader(Handle<Shader> handle);

    Handle<PipelineState> CreatePipelineState(in CreatePipelineStateArgs args);
    void DestroyPipelineState(Handle<PipelineState> handle);
    PipelineState* AccessPipelineState(Handle<PipelineState> handle);

    Handle<RootSignature> CreateRootSignature(in CreateRootSignatureArgs args);
    void DestroyRootSignature(Handle<RootSignature> handle);
    RootSignature* AccessRootSignature(Handle<RootSignature> handle);


    
}
