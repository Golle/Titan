using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Common;
using Titan.Core.Logging;
using Titan.Core.Threading;
using Titan.Graphics.D3D11;
using Titan.Graphics.Materials;
using Titan.Graphics.Meshes;

namespace Titan.Graphics.Textures.NewTest
{

    // Load resource -> return handle for the type
    // Queue a job that will load the resource async ->
    // Increase reference count when resource is loaded
    // Accessing a resource that has not been loaded will have undefined behaviour
    // IsLoaded(handle ) => true|false

    // Release -> decrease reference counter

    public class TestMaterialManager : ResourceManager<Material1[]>
    {
        private readonly IMaterialsLoader _materialsLoader;

        public TestMaterialManager(IMaterialsLoader materialsLoader, WorkerPool pool, uint maxResources) 
            : base(pool, maxResources)
        {
            _materialsLoader = materialsLoader;
        }

        protected override Material1[] OnLoad(string identifier)
        {
            var configuration = _materialsLoader.LoadMaterials(identifier);
            return null;


        }
    }

    public struct Material1
    {
        public Handle<Texture1> NormalMap;
        public Handle<Texture1> DiffuseMap;

        public Color Diffuse;
        public Color Ambient;
        public Color Specular;
        public Color Emissive;

        public bool IsTextured;
        public bool HasNormalMap;
        public bool IsTransparent;
        private bool NOOP;
    }


    public class TestMeshManager : ResourceManager<Mesh[]>
    {
        private readonly IMeshLoader _meshLoader;
        internal TestMeshManager(IMeshLoader meshLoader, WorkerPool pool, uint maxResources) 
            : base(pool, maxResources)
        {
            _meshLoader = meshLoader;
        }

        protected override Mesh[] OnLoad(string identifier) => _meshLoader.LoadMesh(identifier);
    }
    
    public class TestTextureManager : ResourceManager<Texture1>
    {
        private readonly TextureFactory _textureFactory;

        internal TestTextureManager(TextureFactory textureFactory, WorkerPool pool) 
            : base(pool, 100_000)
        {
            _textureFactory = textureFactory;
        }
        protected override Texture1 OnLoad(string identifier) => _textureFactory.CreateFromFile(identifier);
    }

    public abstract class ResourceManager<TResource>
    {
        private static int _nextHandle;

        private readonly WorkerPool _pool;

        private readonly TResource[] _resources;
        private readonly bool[] _loaded;
        private readonly string[] _resourceMap;

        protected ResourceManager(WorkerPool pool, uint maxResources)
        {
            _pool = pool;
            _resources = new TResource[maxResources];
            _loaded = new bool[maxResources];
            _resourceMap = new string[maxResources];
        }

        public Handle<TResource> LoadSync(string identifier)
        {
            int handle;
            lock (_resourceMap)
            {
                for (var i = 0; i <= _nextHandle; ++i)
                {
                    if (_resourceMap[i] == identifier)
                    {
                        return i;
                    }
                }
                handle = Interlocked.Increment(ref _nextHandle);
                _resourceMap[handle] = identifier;
                Volatile.Write(ref _loaded[handle], false);
            }
            LOGGER.Debug("Loading (Sync) {0} from : {1}", typeof(TResource).Name, identifier);
            _resources[handle] = OnLoad(identifier);
            Volatile.Write(ref _loaded[handle], true);
            return handle;
        }

        public Handle<TResource> LoadAsync(string identifier)
        {
            int handle;
            lock (_resourceMap)
            {
                for (var i = 0; i <= _nextHandle; ++i)
                {
                    if (_resourceMap[i] == identifier)
                    {
                        return i;
                    }
                }
                handle = Interlocked.Increment(ref _nextHandle);
                _resourceMap[handle] = identifier;
                Volatile.Write(ref _loaded[handle], false);
            }
            
            _pool.Enqueue(new JobDescription(() =>
            {
                LOGGER.Debug("Loading (Async) {0} from : {1}", typeof(TResource).Name, identifier);
                var resource = OnLoad(identifier);
                _resources[handle] = resource;
                Volatile.Write(ref _loaded[handle], true);
            }));
            return handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsLoaded(in Handle<TResource> handle) => _loaded[handle];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly TResource Get(in Handle<TResource> handle) => ref _resources[handle];
        protected abstract TResource OnLoad(string identifier);

    }



}
