using static Titan.Windows.D3D11.D3D11_FILTER;

namespace Titan.Graphics.D3D11.Samplers
{
    public enum TextureFilter
    {
        MinMagMipPoint = D3D11_FILTER_MIN_MAG_MIP_POINT,
        MinMagPointMipLinear = D3D11_FILTER_MIN_MAG_POINT_MIP_LINEAR,
        MinPointMagLinearMipPoint = D3D11_FILTER_MIN_POINT_MAG_LINEAR_MIP_POINT,
        MinPointMagMipLinear = D3D11_FILTER_MIN_POINT_MAG_MIP_LINEAR,
        MinLinearMagMipPoint = D3D11_FILTER_MIN_LINEAR_MAG_MIP_POINT,
        MinLinearMagPointMipLinear = D3D11_FILTER_MIN_LINEAR_MAG_POINT_MIP_LINEAR,
        MinMagLinearMipPoint = D3D11_FILTER_MIN_MAG_LINEAR_MIP_POINT,
        MinMagMipLinear = D3D11_FILTER_MIN_MAG_MIP_LINEAR,
        Anisotropic = D3D11_FILTER_ANISOTROPIC
    }
}
