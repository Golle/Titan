using System.Runtime.CompilerServices;

namespace Titan.Core.Math
{
    /// <summary>
    /// Implementation based on https://easings.net/
    /// </summary>
    public static class Easing
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseOutCirc(float x) => MathF.Sqrt(1f - MathF.Pow(x - 1f, 2f));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseOutSine(float x) => MathF.Sin((x * MathF.PI) / 2);
    }
}
