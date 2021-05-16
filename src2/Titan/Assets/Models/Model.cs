using System.Runtime.CompilerServices;

namespace Titan.Assets.Models
{
    public class Model
    {
        private readonly Mesh _mesh;
        public ref readonly Mesh Mesh
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _mesh;
        }

        public Model(in Mesh mesh)
        {
            _mesh = mesh;
        }
    }
}
