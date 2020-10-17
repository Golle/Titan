using System;
using System.Numerics;
using Titan.Core;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;

namespace Titan.Graphics.D3D11
{
    public unsafe class InputLayout : IDisposable
    {
        private ComPtr<ID3D11InputLayout> _inputLayout;
        internal ref readonly ComPtr<ID3D11InputLayout> Pointer => ref _inputLayout;

        public InputLayout(IGraphicsDevice device, CompiledShader vertexShader, in InputLayoutDescriptor[] descriptors)
        {
            var inputDescs = stackalloc D3D11_INPUT_ELEMENT_DESC[descriptors.Length];
            var size = 0u;
            for (var i = 0; i < descriptors.Length; ++i)
            {
                ref var descriptor = ref descriptors[i];
                ref var desc = ref inputDescs[i];
                fixed (byte* name = descriptor.Name.AsBytes())
                {
                    desc.SemanticName = (sbyte*)name;
                }
                
                // This needs some changes to support it properly
                desc.InstanceDataStepRate = descriptor.Classification == D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_INSTANCE_DATA ? 1 : 0;
                desc.InputSlot = descriptor.Classification == D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_INSTANCE_DATA ? 1 : 0;
                desc.InputSlotClass = descriptor.Classification;
                desc.SemanticIndex = descriptor.Classification == D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_INSTANCE_DATA ? 1 : 0;
                desc.AlignedByteOffset = descriptor.Classification == D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_INSTANCE_DATA ? 0 : size;
                desc.Format = descriptor.Format;
                
                size += GetSize(descriptor.Format);
            }
            
            CheckAndThrow(device.Ptr->CreateInputLayout(inputDescs, (uint)descriptors.Length, vertexShader.Buffer, vertexShader.BufferSize, _inputLayout.GetAddressOf()), "CreateInputLayout");
        }

        private static uint GetSize(DXGI_FORMAT format) =>
            (uint) (format switch
            {
                DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT => sizeof(Vector2),
                DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT => sizeof(Vector3),
                DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT => sizeof(Vector4),
                _ => throw new NotSupportedException($"Format {format} is not supported.")
            });

        public void Dispose() => _inputLayout.Dispose();
    }
}
