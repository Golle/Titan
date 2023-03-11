using System.Runtime.CompilerServices;

namespace Titan.Core.Maths;

public static class Easings
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float EaseInCubic(float value)
        => value * value * value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float EasyOutCubic(float value)
        => 1f - (float)Math.Pow(1f - value, 3f);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float EaseOutSine(float value)
        => (float)Math.Sin(value * Math.PI / 2f);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double EaseOutSine(double value)
        => Math.Sin(value * Math.PI / 2f);
}
