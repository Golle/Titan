using System.Numerics;
using System.Runtime.InteropServices;
using Titan.Tools.Core.Models.WavefrontObj;

namespace Titan.Tools.Core.Models;

public record ReadModelResult(bool Success, string? Error)
{
    public static ReadModelResult Fail(string error) => new(false, error);
    public static ReadModelResult Ok() => new(true, null);
}


[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct Vertex
{
    public Vector3 Position;
    public Vector3 Normal;
    public Vector2 UV;
}

public class ModelData
{
    public required int[] Indices;
    public required int IndexSize;
    public required Vertex[] Data;
}

public static class ModelConverter
{
    public static ReadModelResult  ReadModel(string path)
    {
        if (!File.Exists(path))
        {
            return ReadModelResult.Fail($"Failed to find a file at path {path}");
        }

        if (!Path.GetExtension(path).Equals(".obj", StringComparison.InvariantCultureIgnoreCase))
        {
            return ReadModelResult.Fail("Only Obj files are supported at this time.");
        }

        var obj = ObjParser.Load(path);
        if (obj == null)
        {
            return ReadModelResult.Fail("Failed to read the obj file.");
        }


        throw new NotImplementedException("This is not the way we want this to work :|");


        return ReadModelResult.Ok();

    }
}
