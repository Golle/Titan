using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Graphics.Textures;

namespace Titan.Graphics.Materials
{
    // TODO: Implement hot relaoding of materials (Only in Debug mode, maybe another implementation of the material manager?)
    internal unsafe class MaterialsManager : IMaterialsManager
    {
        private readonly ITextureLoader _textureLoader;

        private readonly Material* _materials;
        private readonly uint _maxMaterials;

        private int _numberOfMaterials;
        
        public MaterialsManager(ITextureLoader textureLoader, IMemoryManager memoryManager)
        {
            _textureLoader = textureLoader;
            var memory = memoryManager.GetMemoryChunkValidated<Material>("Materials");
            _materials = memory.Pointer;
            _maxMaterials = memory.Count;
        }
        
        public MaterialHandle CreateFromConfiguration(in MaterialConfiguration materialConfiguration)
        {
            var handle = Interlocked.Increment(ref _numberOfMaterials) - 1;
            if (handle >= _maxMaterials)
            {
                throw new InvalidOperationException($"Max number of materials has been reached. {_maxMaterials}");
            }
            _materials[handle] = new Material
            {
                Ambient = Color.TryParse(materialConfiguration.Ambient, out var ambient) ? ambient : Color.Zero,
                Diffuse = Color.TryParse(materialConfiguration.Diffuse, out var diffuse) ? diffuse : Color.White,
                DiffuseMap = LoadTexture(materialConfiguration.DiffuseMap),
                Emissive = Color.TryParse(materialConfiguration.Emissive, out var emissive) ? emissive : Color.Zero,
                Specular = Color.TryParse(materialConfiguration.Specular, out var specular) ? specular : Color.Zero,
                NormalMap = LoadTexture(materialConfiguration.NormalMap),
                IsTransparent = materialConfiguration.IsTransparent,
                IsTextured = !string.IsNullOrWhiteSpace(materialConfiguration.DiffuseMap),
                HasNormalMap = !string.IsNullOrWhiteSpace(materialConfiguration.NormalMap),
            };
            return handle;
        }
        private Texture LoadTexture(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return default;
            }

            return _textureLoader.Load(filename);
        }

        public ref readonly Material this[in MaterialHandle handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _materials[handle];
        }

        public void Destroy(in MaterialHandle handle)
        {
            ref var material = ref _materials[handle];
            if (material.IsTextured)
            {
                _textureLoader.Release(material.DiffuseMap);
            }
            if (material.HasNormalMap)
            {
                _textureLoader.Release(material.NormalMap);
            }
            material = default;
            // TODO: return the handle so it can be re-used.
        }
    }
}
