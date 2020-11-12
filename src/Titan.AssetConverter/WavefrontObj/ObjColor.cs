namespace Titan.AssetConverter.WavefrontObj
{
    public readonly struct ObjColor
    {
        public readonly float Red;
        public readonly float Green;
        public readonly float Blue;
        public ObjColor(float r, float g, float b)
        {
            Red = r;
            Blue = b;
            Green = g;
        }
    }
}