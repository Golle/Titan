using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Titan.Core.Common;
using Titan.Core.IO;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Buffers;
using Titan.GraphicsV2.D3D11.Textures;
using Titan.GraphicsV2.Resources.Materials;
using Titan.GraphicsV2.Resources.Models;
using Titan.GraphicsV2.Resources.Textures;
using Titan.Windows.Win32.D3D11;
using Buffer = Titan.GraphicsV2.D3D11.Buffers.Buffer;

namespace Titan.GraphicsV2.Resources.Bundles
{

    internal record BundleDescriptor
    {
        public int Size { get; init; }
        public MeshDescriptor[] Meshes { get; init; }
        public TextureDescriptor[] Textures { get; init; }
    }

    internal record TextureDescriptor
    {
        public string Name { get; init; }
        public long Offset { get; init; }
        public int Size { get; init; }
    }

    internal record MeshDescriptor
    {
        public string Name { get; init; }
        public long Offset { get; init; }
        public int VertexSize { get; init; }
        public int VertexCount { get; init; }
        public int IndexSize { get; init; }
        public int IndexCount { get; init; }
        public int SubmeshSize { get; init; }
        public int SubmeshCount { get; init; }
        public MaterialDescriptor Materials { get; init; }
    }

    internal record MaterialDescriptor
    {
        public int Count { get; init; }
    }

    internal class BundleLoader
    {
        private readonly IFileReader _fileReader;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly Device _device;
        private readonly IImageLoader _imageLoader;

        public BundleLoader(IFileReader fileReader, IJsonSerializer jsonSerializer, Device device, IImageLoader imageLoader)
        {
            _fileReader = fileReader;
            _jsonSerializer = jsonSerializer;
            _device = device;
            _imageLoader = imageLoader;
        }

        public unsafe Bundle Load(string identifier)
        {
            var content = _fileReader.ReadText($"{identifier}.json");
            var bundle = _jsonSerializer.Deserialize<BundleDescriptor>(content);
            using var file = _fileReader.OpenRead($"{identifier}.dat");

            var pData = (byte*) Marshal.AllocHGlobal(bundle.Size);
            file.Read(new Span<byte>(pData, bundle.Size));
            List<Handle<Texture>> textures = new(bundle.Textures.Length);
            List<Model3D> models = new();
            
            try
            {
                foreach (var textureDescriptor in bundle.Textures)
                {
                    textures.Add(CreateTexture(textureDescriptor, pData));
                }

                foreach (var meshDescriptor in bundle.Meshes)
                {
                    models.Add(CreateModel(textures, meshDescriptor, pData));
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint) pData);
            }

            return new Bundle
            {
                Models = models.ToArray(),
                Textures = textures.ToArray()
            };
        }

        private unsafe Handle<Texture> CreateTexture(TextureDescriptor descriptor, byte* pData)
        {
            var pTexture = pData + descriptor.Offset;
            using var image = _imageLoader.Load(pTexture, (uint) descriptor.Size);
            return _device.TextureManager.Create(new TextureCreation
            {
                Binding = TextureBindFlags.ShaderResource,
                DataStride = image.Stride,
                Format = image.Format,
                Height = image.Height,
                Width = image.Width,
                InitialData = new DataBlob(image.GetBuffer()),
                Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE
            });
        }

        private unsafe Model3D CreateModel(List<Handle<Texture>> textures, MeshDescriptor descriptor, byte* pData)
        {
            var pMesh = pData + descriptor.Offset;

            var verticesSize = descriptor.VertexCount * descriptor.VertexSize;
            var indicesSize = descriptor.IndexCount * descriptor.IndexSize;
            var submeshSize = descriptor.SubmeshCount * descriptor.SubmeshSize;
            var meshSize = verticesSize + indicesSize + submeshSize;

            var vertexBuffer = _device.BufferManager.Create(new BufferCreation
            {
                Stride = (uint) descriptor.VertexSize,
                Count = (uint) descriptor.VertexCount,
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                InitialData = new DataBlob(pMesh),
                Type = BufferTypes.VertexBuffer,
                Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE
            });

            var indexBuffer = _device.BufferManager.Create(new BufferCreation
            {
                Count = (uint) descriptor.IndexCount,
                Stride = (uint) descriptor.IndexSize,
                Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE,
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                InitialData = new DataBlob(pMesh + verticesSize),
                Type = BufferTypes.IndexBuffer
            });


            var pSubmeshes = (SubMeshData*) (pMesh + verticesSize + indicesSize);
            var pMaterials = (MaterialData*) (pMesh + meshSize);

            var submeshes = new SubMeshTemp[descriptor.SubmeshCount];

            Handle<Texture> GetTexture(int index) => index != -1 ? textures[index] : Handle<Texture>.Null;

            for (var i = 0; i < descriptor.SubmeshCount; ++i)
            {
                var submeshData = pSubmeshes[i];
                ref var submesh = ref submeshes[i];
                submesh.Count = submeshData.Count;
                submesh.Start = submeshData.StartIndex;
                submesh.HasMaterial = submeshData.MaterialIndex != -1;
                if (submesh.HasMaterial)
                {
                    ref readonly var materialData = ref pMaterials[i];
                    submesh.Material = new MaterialTemp
                    {
                        DiffuseTexture = GetTexture(materialData.DiffuseTextureIndex),
                        Properties = new MaterialProperties
                        {
                            Ambient = materialData.Ambient,
                            Specular = materialData.Specular,
                            Emissive = materialData.Emissive,
                            Diffue = materialData.Diffuse
                        }
                    };
                }
            }
          
            return new Model3D
            {
                SubMeshes = submeshes,
                IndexBuffer = indexBuffer,
                VertexBuffer = vertexBuffer
            };
        }
    }

    internal struct Bundle
    {
        internal Model3D[] Models;
        internal Handle<Texture>[] Textures;
    }

    struct SubMeshTemp
    {
        internal uint Start;
        internal uint Count;
        internal bool HasMaterial;
        internal MaterialTemp Material;
    }

    internal struct MaterialTemp
    {
        internal Handle<Texture> DiffuseTexture;
        internal MaterialProperties Properties;
    }

    internal struct Model3D
    {
        internal Handle<Buffer> VertexBuffer;
        internal Handle<Buffer> IndexBuffer;
        internal SubMeshTemp[] SubMeshes;

    }
}
