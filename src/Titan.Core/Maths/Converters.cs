using System.Runtime.CompilerServices;

namespace Titan.Core.Maths;

public static class Converters
{
    public static readonly double DegreeRadianConstant = Math.PI / 180.0;
    public static readonly float DegreeRadianConstantF = (float)(Math.PI / 180.0f);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DegressToRadians(float value)
        => value * DegreeRadianConstantF;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double DegressToRadians(double value)
        => value * DegreeRadianConstant;
}
