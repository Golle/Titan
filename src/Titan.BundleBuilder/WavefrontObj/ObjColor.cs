using System;

namespace Titan.BundleBuilder.WavefrontObj
{
    public readonly struct ObjColor
    {
        public readonly float Red;
        public readonly float Green;
        public readonly float Blue;
        public readonly string Original;
        public ObjColor(in ReadOnlySpan<string> original, float r, float g, float b)
        {
            Original = string.Join(' ', original.ToArray());
            Red = r;
            Blue = b;
            Green = g;
        }

        public bool HasValue => !string.IsNullOrWhiteSpace(Original);

        public override string ToString() => Original;
    }
}
