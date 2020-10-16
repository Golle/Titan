using System;
using System.Runtime.CompilerServices;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.D3D11
{
    public unsafe class Texture2D : IResource, IDisposable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ID3D11Resource* AsResourcePointer() => (ID3D11Resource*) _texture2D.Get();
        public DXGI_FORMAT Format { get; }
        public ref readonly ComPtr<ID3D11Texture2D> Ptr => ref _texture2D;
        private  ComPtr<ID3D11Texture2D> _texture2D;

        public Texture2D(IGraphicsDevice device, uint width, uint height, IntPtr buffer, uint size) 
            : this(device, width, height,(byte*) buffer.ToPointer(), size)
        {
        }

        public Texture2D(IGraphicsDevice device, uint width, uint height, byte* buffer, uint size)
        {
            D3D11_TEXTURE2D_DESC desc;
            desc.Width = width;
            desc.Height = height;
            desc.Format = Format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;
            desc.BindFlags = D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE;
            desc.CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED;
            desc.Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT;
            desc.MiscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFICED;
            desc.MipLevels = 1;
            desc.ArraySize = 1;
            desc.SampleDesc.Count = 1;
            desc.SampleDesc.Quality = 0;

            D3D11_SUBRESOURCE_DATA data;
            data.pSysMem = buffer;
            data.SysMemPitch = width * 4;

            Common.CheckAndThrow(device.Ptr->CreateTexture2D(&desc, &data, _texture2D.GetAddressOf()), "CreateTexture2D");
        }

        public Texture2D(IGraphicsDevice device, uint width, uint height)
        {
            D3D11_TEXTURE2D_DESC desc;
            desc.Width = width;
            desc.Height = height;
            desc.Format = Format = DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT;
            desc.BindFlags = D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET | D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE;
            desc.CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED;
            desc.Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT;
            desc.MiscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFICED;
            desc.MipLevels = 1;
            desc.ArraySize = 1;
            desc.SampleDesc.Count = 1;
            desc.SampleDesc.Quality = 0;

            Common.CheckAndThrow(device.Ptr->CreateTexture2D(&desc, null, _texture2D.GetAddressOf()), "CreateTexture2D");
        }



        public void Dispose() => _texture2D.Dispose();
    }
}
