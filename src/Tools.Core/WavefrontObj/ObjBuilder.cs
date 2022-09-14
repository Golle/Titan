using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tools.Core.WavefrontObj
{
    internal class ObjBuilder
    {
        private readonly List<Vector3> _vertices = new();
        private readonly List<Vector3> _normals = new();
        private readonly List<Vector2> _textures = new();

        private ObjMaterial[] _materials;

        private ObjGroup _currentGroup;

        private readonly List<ObjGroup> _groups = new();

        private int _smoothGroup;
        private int _currentMaterialIndex = -1;

        internal void AddPosition(in ReadOnlySpan<string> position)
        {
            if (position.Length != 3)
            {
                throw new FormatException("Vertices must be of format v x y z");
            }
            _vertices.Add(ParseVector3(position));
        }

        internal void AddTexture(in ReadOnlySpan<string> texture)
        {
            if (texture.Length < 2 || texture.Length > 3)
            {
                throw new FormatException("Textures must be of format vt x y [w]");
            }
            _textures.Add(ParseVector2(texture));
        }

        internal void AddNormal(in ReadOnlySpan<string> normal)
        {
            if (normal.Length != 3)
            {
                throw new FormatException("Normals must be of format v x y z");
            }
            _normals.Add(ParseVector3(normal));
        }

        internal void AddGroup(in ReadOnlySpan<string> group)
        {
            _groups.Add(_currentGroup = new ObjGroup(group.Length > 0 ? group[0] : null));
        }

        internal void AddSmoothGroup(in ReadOnlySpan<string> smooth)
        {
            if (smooth.Length != 1)
            {
                throw new FormatException("Smooth must be of format s {off|int}");
            }
            _smoothGroup = smooth[0].Equals("off", StringComparison.OrdinalIgnoreCase) ? 0 : int.Parse(smooth[0]);
        }

        public void AddFace(in ReadOnlySpan<string> face)
        {
            if(face.Length < 3)
            {
                throw new FormatException("Face must be of format f {f1} {f2} {f3} [{f4...}]");
            }
            
            var vertices = new ObjVertex[face.Length];

            for (var i = 0; i < face.Length; ++i)
            {
                var values = face[i].Split('/', StringSplitOptions.TrimEntries)
                    .Select(x => int.Parse(x) - 1) // Subtract 1 since .obj file array index starts at 1
                    .ToArray();

                vertices[i] = values.Length switch
                {
                    1 => new ObjVertex(values[0]),
                    2 => new ObjVertex(values[0], values[1]),
                    3 => new ObjVertex(values[0], values[1], values[2]),
                    _ => throw new InvalidOperationException("The format of the face is wrong. Only 1, 2 or 3 parts are supported. V, V/T or V/T/N")
                };
                
            }

            if (_currentGroup == null)
            {
                // If there's no group, create a default unnamed group
                _groups.Add(_currentGroup = new ObjGroup());
            }
            _currentGroup.AddFace(new ObjFace(_currentMaterialIndex, _smoothGroup, vertices));
        }

        public void UseMaterial(in ReadOnlySpan<string> values)
        {
            if(_materials == null)
            {
                throw new InvalidOperationException("Materials is missing.");
            }
            
            var name = values[0];
            _currentMaterialIndex = Array.FindIndex(_materials, m => m.Name == name);
            if (_currentMaterialIndex == -1)
            {
                throw new InvalidOperationException($"Material {name} could not be found.");
            }
        }

        public void SetMaterials(ObjMaterial[] materials)
        {
            if (_materials != null)
            {
                throw new InvalidOperationException("Materials has already been set.");
            }
            _materials = materials;
        }

        internal WavefrontObject Build() => new(_groups.ToArray(), _materials, _vertices.ToArray(), _normals.ToArray(), _textures.ToArray());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3 ParseVector3(in ReadOnlySpan<string> values) => new(ParseFloat(values[0]), ParseFloat(values[1]), ParseFloat(values[2]));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2 ParseVector2(in ReadOnlySpan<string> values) => new(ParseFloat(values[0]), ParseFloat(values[1]));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ParseFloat(in string value) => float.Parse(value, CultureInfo.InvariantCulture);
    }
}
