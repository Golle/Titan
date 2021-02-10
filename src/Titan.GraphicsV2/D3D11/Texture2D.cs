using System.Runtime.CompilerServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    public readonly unsafe struct Texture2D
    {
        private readonly ID3D11Texture2D* _texture;
        public readonly DXGI_FORMAT Format;
        public readonly uint Width;
        public readonly uint Height;

        internal Texture2D(ID3D11Texture2D* texture, DXGI_FORMAT format, uint width, uint height)
        {
            _texture = texture;
            Format = format;
            Width = width;
            Height = height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ID3D11Resource* AsResource() => (ID3D11Resource*) _texture;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ID3D11Texture2D* AsPtr() => _texture;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ID3D11Resource*(in Texture2D texture) => (ID3D11Resource*) texture._texture;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ID3D11Texture2D*(in Texture2D texture) => texture._texture;

        public void Release() => _texture->Release();
    }
}
