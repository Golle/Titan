using System.Numerics;
// ReSharper disable InconsistentNaming

namespace Titan.Core.Common
{
    public class MatrixExtensions
    {
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
