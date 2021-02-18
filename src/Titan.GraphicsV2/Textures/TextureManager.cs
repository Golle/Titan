using Titan.Core.Common;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Shaders;
using Titan.GraphicsV2.Resources;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Textures
{

    
    internal struct Model
    {
        public Mesh Mesh;
        
    }


    internal struct Mesh
    {
        public Buffer VertexBuffer;
        public Buffer IndexBuffer;
        public uint Indices;
        public D3D_PRIMITIVE_TOPOLOGY Primitive;
    }


    public class ModelManager
    {
        private readonly TextureManager _textureManager;
        //private readonly MaterialManager _materialManager;
        //private readonly MeshManager _meshManager;

        //private Model[] _loadedModels = new Model[100];
        //public ModelManager(TextureManager textureManager, MaterialManager materialManager, MeshManager meshManager)
        //{
        //    _textureManager = textureManager;
        //    _materialManager = materialManager;
        //    _meshManager = meshManager;
        //}


        //public Handle<int> LoadFromFile(string identifier)
        //{

        //    var material = _materialManager.Load(identifier);
        //    var mesh = _meshManager.Load(identifier);

            
        //}
    }
    
    
    
    public class TextureManager
    {
        public Handle<int> LoadTextureFromFileAsync(string filename)
        {

            return 0;
        }
        
        public Handle<int> LoadTextureFromFile(string filename)
        {
            return 0;
        }

    }
}
