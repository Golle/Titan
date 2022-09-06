using System;
using System.Diagnostics;
using Titan.Assets.NewAssets;
using Titan.Core;
using Titan.Memory;
using Titan.Memory.Arenas;

namespace Titan.Graphics.LoadersNEW;

public struct Shader
{

}

internal unsafe struct ShaderCreator : IResourceCreator<Shader>
{
    private const uint _initialCount = 100;
    private DynamicPoolArena _shaders;

    public bool Init(PlatformAllocator* allocator)
    {
        _shaders = DynamicPoolArena.Create(allocator, 100, (uint)sizeof(Shader));
        return true;
    }

    public static unsafe Handle<Shader> Create(void* context, ReadOnlySpan<byte> data)
    {
        var creator = (ShaderCreator*)context;

        //creator.
        throw new NotImplementedException();
    }

    public static unsafe void Destroy(void* context, Handle<Shader> handle)
    {
        throw new NotImplementedException();
    }
}
