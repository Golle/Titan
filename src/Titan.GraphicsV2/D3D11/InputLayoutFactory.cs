using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Titan.Core;
using Titan.GraphicsV2.D3D11.Shaders;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.D3D11.DXGI_FORMAT;

namespace Titan.GraphicsV2.D3D11
{
    public record InputLayoutDescriptor(string Name, DXGI_FORMAT Format, uint Slot = 0, D3D11_INPUT_CLASSIFICATION Classification = D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_VERTEX_DATA);

    internal unsafe class InputLayoutFactory
    {
        private readonly Device _device;
        public InputLayoutFactory(Device device)
        {
            _device = device;
        }

        internal ID3D11InputLayout* CreateP(in CompiledShader shader, InputLayoutDescriptor[] descriptors) => Create(shader, descriptors);

        public InputLayout Create(in CompiledShader shader, InputLayoutDescriptor[] descriptors)
        {
            static uint GetSize(DXGI_FORMAT format) =>
                (uint)(format switch
                {
                    DXGI_FORMAT_R32G32_FLOAT => sizeof(Vector2),
                    DXGI_FORMAT_R32G32B32_FLOAT => sizeof(Vector3),
                    DXGI_FORMAT_R32G32B32A32_FLOAT => sizeof(Vector4),
                    _ => throw new NotSupportedException($"Format {format} is not supported.")
                });


            var count = descriptors.Length;

            var descs = stackalloc D3D11_INPUT_ELEMENT_DESC[count];
            var size = 0u;
            for (var i = 0u; i < count; ++i)
            {
                // TODO: add support for GPU instancing
                var (name, format, slot, classification) = descriptors[i];
                descs[i] = new D3D11_INPUT_ELEMENT_DESC
                {
                    AlignedByteOffset = size,
                    Format = format,
                    InputSlot = slot,
                    InputSlotClass = classification,
                    SemanticIndex = 0,
                    InstanceDataStepRate = 0,
                };
                var length = name.Length;
                descs[i].SemanticName = (sbyte*)Marshal.AllocHGlobal(length + 1);
                descs[i].SemanticName[length] = 0;
                fixed (byte* pName = name.AsBytes())
                {
                    System.Buffer.MemoryCopy(pName, descs[i].SemanticName, length, length);
                }
                size += GetSize(format);
            }

            ID3D11InputLayout* inputLayout;
            try
            {
                CheckAndThrow(_device.Get()->CreateInputLayout(descs, (uint)count, shader.GetBuffer(), shader.GetSize(), &inputLayout), nameof(ID3D11Device.CreateInputLayout));
            }
            finally
            {
                for (var i = 0; i < count; i++)
                {
                    Marshal.FreeHGlobal((nint)descs[i].SemanticName);
                }
            }
            
            return new (inputLayout);
        }
    }
}
