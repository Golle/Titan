using static Titan.Platform.Win32.D3D11.D3D11_COMPARISON_FUNC;

namespace Titan.Graphics.D3D11.Samplers
{
    public enum ComparisonFunc
    {
        Never = D3D11_COMPARISON_NEVER,
        Less = D3D11_COMPARISON_LESS,
        Equal = D3D11_COMPARISON_EQUAL,
        LessEqual = D3D11_COMPARISON_LESS_EQUAL,
        Greater = D3D11_COMPARISON_GREATER,
        GreaterNotEqual = D3D11_COMPARISON_NOT_EQUAL,
        GreaterEqual = D3D11_COMPARISON_GREATER_EQUAL,
        Always = D3D11_COMPARISON_ALWAYS
    }
}
