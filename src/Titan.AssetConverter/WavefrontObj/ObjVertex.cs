namespace Titan.AssetConverter.WavefrontObj
{
    public readonly struct ObjVertex
    {
        public readonly int VertexIndex;
        public readonly int TextureIndex;
        public readonly int NormalIndex;

        public ObjVertex(int vertexIndex, int textureIndex = -1, int normalIndex = -1)
        {
            VertexIndex = vertexIndex;
            TextureIndex = textureIndex;
            NormalIndex = normalIndex;
        }
    }
}
