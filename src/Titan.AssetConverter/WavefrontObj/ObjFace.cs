namespace Titan.AssetConverter.WavefrontObj
{
    public class ObjFace
    {
        public int Material { get; }
        public int SmoothGroup { get; }
        public ObjVertex[] Vertices { get; }  
        public ObjFace(int material, int smoothGroup, in ObjVertex[] vertices)
        {
            Material = material;
            SmoothGroup = smoothGroup;
            Vertices = vertices;
        }
    }
}
