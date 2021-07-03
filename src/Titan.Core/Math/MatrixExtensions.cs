using System.Numerics;

namespace Titan.Core.Math
{
    public class MatrixExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static Matrix4x4 CreatePerspectiveLH(float width, float height, float nearPlaneDistance, float farPlaneDistance)
        {
            Matrix4x4 matrix = default;
            matrix.M11 = 2 * nearPlaneDistance / width;
            matrix.M22 = 2 * nearPlaneDistance / height;
            matrix.M33 = farPlaneDistance / (farPlaneDistance - nearPlaneDistance);
            matrix.M34 = 1f;
            matrix.M43 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            //matrix.M43 = -nearPlaneDistance * farPlaneDistance / (farPlaneDistance - nearPlaneDistance);
            return matrix;
        }
    }
}
