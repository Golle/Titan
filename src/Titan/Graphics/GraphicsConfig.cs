using Titan.Core.Maths;
using Titan.Core.Memory;
using Titan.Setup;
using Titan.Setup.Configs;

namespace Titan.Graphics;

public record struct GPUMemoryConfig(uint SRVCount, uint RTVCount, uint DSVCount, uint UAVCount, uint TempConstantBufferSize, uint TempSRVCount);
public record struct GraphicsResourcesConfig(uint MaxBuffers, uint MaxTextures, uint MaxPipelineStates, uint UploadFrames, uint MaxShaders, uint MaxResourceBuffersSize, uint MaxRootSignatures);
public record GraphicsConfig(bool Debug, bool TripleBuffering, bool Vsync, bool AllowTearing, bool Fullscreen, Color ClearColor) : IConfiguration, IDefault<GraphicsConfig>
{
    public static readonly uint DefaultMaxResourceBuffersSize = MemoryUtils.MegaBytes(4);

    public const uint DefaultMaxBuffers = 1000;
    public const uint DefaultMaxTextures = 1000;
    public const uint DefaultMaxPipelines = 100;
    public const uint DefaultUploadFrames = 4;
    public const uint DefaultMaxShaders = 128;
    public const uint DefaultMaxRootSignatures = 16;

    public const uint DefaultSRVCount = 1024;
    public const uint DefaultRTVCount = 1024;
    public const uint DefaultDSVCount = 1024;
    public const uint DefaultUAVCount = 1024;
    public const uint DefaultTempSRVCount = 256;
    public static readonly uint DefaultTempConstantBufferSize = MemoryUtils.MegaBytes(2);

    public static readonly Color DefaultClearColor = Color.FromRGB(0x001f54);

    public GraphicsResourcesConfig ResourcesConfig { get; init; }
    public GPUMemoryConfig MemoryConfig { get; init; }
    public static GraphicsConfig Default => new(false, true, false, true, false, DefaultClearColor)
    {
        ResourcesConfig = new(DefaultMaxBuffers, DefaultMaxTextures, DefaultMaxPipelines, DefaultUploadFrames, DefaultMaxShaders, DefaultMaxResourceBuffersSize, DefaultMaxRootSignatures),
        MemoryConfig = new(DefaultSRVCount, DefaultRTVCount, DefaultDSVCount, DefaultUAVCount, DefaultTempConstantBufferSize, DefaultTempSRVCount)
    };
}
