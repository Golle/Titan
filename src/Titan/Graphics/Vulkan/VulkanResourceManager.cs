using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics.Resources;

namespace Titan.Graphics.Vulkan;

internal class VulkanResourceManager : IResourceManager
{
    public Handle<Texture> CreateTexture(in CreateTextureArgs args)
    {
        throw new NotImplementedException();
    }

    public bool UploadTexture(in Handle<Texture> handle, TitanBuffer buffer)
    {
        throw new NotImplementedException();
    }

    public void Destroy(Handle<Texture> handle)
    {
        throw new NotImplementedException();
    }

    public unsafe Texture* AccessTexture(Handle<Texture> handle)
    {
        throw new NotImplementedException();
    }

    public Handle<Model3D> CreateModel(in CreateModel3DArgs args)
    {
        throw new NotImplementedException();
    }

    public void DestroyModel(Handle<Model3D> handle)
    {
        throw new NotImplementedException();
    }

    public unsafe Model3D* AccessModel(Handle<Model3D> handle)
    {
        throw new NotImplementedException();
    }

    public Handle<GPUBuffer> CreateBuffer(in CreateBufferArgs args)
    {
        throw new NotImplementedException();
    }

    public void DestroyBuffer(Handle<GPUBuffer> handle)
    {
        throw new NotImplementedException();
    }

    public unsafe GPUBuffer* AccessBuffer(Handle<GPUBuffer> handle)
    {
        throw new NotImplementedException();
    }

    public Handle<Shader> CreateShader(in CreateShaderArgs args)
    {
        throw new NotImplementedException();
    }

    public void DestroyShader(Handle<Shader> handle)
    {
        throw new NotImplementedException();
    }

    public unsafe Shader* AccessShader(Handle<Shader> handle)
    {
        throw new NotImplementedException();
    }

    public Handle<PipelineState> CreatePipelineState(in CreatePipelineStateArgs args)
    {
        throw new NotImplementedException();
    }

    public void DestroyPipelineState(Handle<PipelineState> handle)
    {
        throw new NotImplementedException();
    }

    public unsafe PipelineState* AccessPipelineState(Handle<PipelineState> handle)
    {
        throw new NotImplementedException();
    }

    public Handle<RootSignature> CreateRootSignature(in CreateRootSignatureArgs args)
    {
        throw new NotImplementedException();
    }

    public void DestroyRootSignature(Handle<RootSignature> handle)
    {
        throw new NotImplementedException();
    }

    public unsafe RootSignature* AccessRootSignature(Handle<RootSignature> handle)
    {
        throw new NotImplementedException();
    }
}
