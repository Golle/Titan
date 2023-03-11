#define ASEPRITE_TRACE
using System.Diagnostics;
using System.IO.Compression;
using Titan.Core.Logging;
using static Titan.Tools.Core.Images.Aseprite.FLIConstants;

namespace Titan.Tools.Core.Images.Aseprite;

public record AsepriteFile(uint ColorDepth, uint Width, uint Height, Frame[] Frames)
{
    public uint BytesPerPixel => ColorDepth / 8;
}
public record Layer(uint Width, uint Height, LayerFlags Flags, int Type, int ChildLevel, BlendMode BlendMode, byte Opacity, byte[] PixelData);
public record Frame(Layer[] Layers);

public unsafe class AsepriteReader
{
    public AsepriteFile Read(string path)
    {
        var file = File.ReadAllBytes(path);
        //using var handle = File.OpenHandle(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, FileOptions.SequentialScan);
        //var length = RandomAccess.GetLength(handle);
        //var bytes = stackalloc byte[(int)length];

        //var bytesRead = RandomAccess.Read(handle, new Span<byte>(bytes, (int)length), 0);
        //Debug.Assert(bytesRead == length);
        fixed (byte* pbytes = file)
        {
            var bytes = pbytes;
            var header = (AsepriteHeader*)bytes;
            Trace($"Reading aseprite file with Width = {header->Width}, Height = {header->Height}, ColorDepth = {header->ColorDepth}, Palette count = {header->NumberOfColors}");

            // move to the first frame
            bytes += sizeof(AsepriteHeader);

            var frames = new Frame[header->Frames];
            for (var frameCount = 0; frameCount < header->Frames; frameCount++)
            {
                Trace($"Reading frame index: {frameCount}");
                var frame = ReadFrame((AsepriteFrame*)bytes);
                frames[frameCount] = frame;
            }
            return new AsepriteFile(header->ColorDepth, header->Width, header->Height, frames);
        }
    }

    private static Frame ReadFrame(AsepriteFrame* frame)
    {
        Debug.Assert(frame->MagicNumber == FLI_FRAME_MAGIC_NUMBER);
        Debug.Assert(frame->NumberOfChunks == frame->NumberOfChunksOld);
        var layers = stackalloc LayerChunk*[100]; // max 100 layers
        var layerCount = 0;
        var frameSize = frame->Size;
        var chunks = frame->NumberOfChunks;

        Trace($"\tFrame. Number of chunks = {chunks}, size in bytes = {frameSize}");
        var ptr = (byte*)(frame + 1);

        List<Layer> combinedLayers = new();
        for (var i = 0; i < chunks; i++)
        {
            var chunk = (AsepriteChunk*)ptr;
            var data = (void*)(chunk + 1);
            Trace($"\tChunk. Size = {chunk->Size}, Type = 0x{(ushort)chunk->Type:X4}, Type name = {chunk->Type}");
            switch (chunk->Type)
            {
                case AsepriteChunkType.Layer:
                    // put the pointers to the chunk in an array the cel can read the layer info from.
                    var pLayer = (LayerChunk*)data;
                    layers[layerCount++] = pLayer;
                    Trace($"\tLayer. Flags = {pLayer->Flags}, Type = {pLayer->Type}, ChildLevel = {pLayer->ChildLevel}, BlendMode = {pLayer->BlendMode}, Opacity = {pLayer->Opacity}");
                    break;
                case AsepriteChunkType.Cel:
                    var cel = (CelChunk*)data;
                    var layer = ReadLayer(layers[cel->LayerIndex], cel, chunk->Size);
                    combinedLayers.Add(layer);
                    break;
            }
            ptr += chunk->Size;
        }
        return new Frame(combinedLayers.ToArray());
    }
    private static Layer ReadLayer(LayerChunk* layer, CelChunk* cel, uint chunkSize)
    {
        byte[] pixelData;
        switch (cel->Type)
        {
            case CelType.CompressedImage:
                var dataPtr = (ushort*)(cel + 1);
                var width = *(dataPtr++);
                var height = *(dataPtr++);
                var data = (byte*)dataPtr;
                var size = chunkSize - sizeof(ushort) * 2 - sizeof(CelChunk);
                Trace($"\t\tCompressed Image. Width = {width}, Height = {height}, Size: {size} bytes");
                {
                    using var zlib = new ZLibStream(new UnmanagedMemoryStream(data, size), CompressionMode.Decompress);
                    using var memStream = new MemoryStream();
                    zlib.CopyTo(memStream);
                    pixelData = memStream.ToArray();
                }
                return new Layer(width, height, layer->Flags, layer->Type, layer->ChildLevel, layer->BlendMode, layer->Opacity, pixelData);
            default:
                throw new NotSupportedException($"Cel type {cel->Type} has not been implemented yet.");
        }

    }

    [Conditional("ASEPRITE_TRACE")]
    private static void Trace(string message)
        => Logger.Trace(message, typeof(AsepriteReader));
}

