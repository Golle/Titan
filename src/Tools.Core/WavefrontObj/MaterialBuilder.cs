using System.Globalization;
using System.Runtime.CompilerServices;

namespace Tools.Core.WavefrontObj
{
    internal class MaterialBuilder
    {
        private readonly IList<ObjMaterial> _materials = new List<ObjMaterial>();
        
        private ObjMaterial _currentMaterial;

        public void NewMaterial(ReadOnlySpan<string> name)
        {
            _materials.Add(_currentMaterial = new ObjMaterial(name[0]));
        }

        public void SetShininess(in ReadOnlySpan<string> values)
        {
            if (values.Length != 1)
            {
                throw new FormatException("Shininess must be in format Ns <float>");
            }
            _currentMaterial.Shininess = ParseFloat(values[0]);
        }

        public void SetAlpha(in ReadOnlySpan<string> values)
        {
            if (values.Length != 1)
            {
                throw new FormatException("Alpha must be in format d <float>");
            }
            _currentMaterial.Alpha = ParseFloat(values[0]);
        }

        public ObjMaterial[] Build()
        {
            return _materials.ToArray();
        }

        public void SetTransparency(in ReadOnlySpan<string> values)
        {
            if (values.Length != 1)
            {
                throw new FormatException("Transparency must be in format Tr <float>");
            }
            _currentMaterial.Transparency = ParseFloat(values[0]);
        }

        public void SetIllumination(in ReadOnlySpan<string> values)
        {
            if (values.Length != 1)
            {
                throw new FormatException("Illumination must be in format illum <int>");
            }
            _currentMaterial.Illumination = int.Parse(values[0]);
        }

        public void SetAmbientColor(in ReadOnlySpan<string> values)
        {
            if (values.Length != 3)
            {
                throw new FormatException("Ambient color must be in format Ka <float> <float> <float>");
            }
            _currentMaterial.AmbientColor = ParseColor(values);
        }

        public void SetDiffuseColor(in ReadOnlySpan<string> values)
        {
            if (values.Length != 3)
            {
                throw new FormatException("Diffuse color must be in format Kd <float> <float> <float>");
            }
            _currentMaterial.DiffuseColor = ParseColor(values);
        }

        public void SetSpecularColor(in ReadOnlySpan<string> values)
        {
            if (values.Length != 3)
            {
                throw new FormatException("Specular color must be in format Ks <float> <float> <float>");
            }
            _currentMaterial.SpecularColor = ParseColor(values);
        }

        public void SetEmissiveColor(in ReadOnlySpan<string> values)
        {
            if (values.Length != 3)
            {
                throw new FormatException("Emissive color must be in format Ke <float> <float> <float>");
            }
            _currentMaterial.EmissiveColor = ParseColor(values);
        }

        public void SetAmbientMap(in ReadOnlySpan<string> values)
        {
            if (values.Length != 1)
            {
                throw new FormatException("Ambient Map must be in format map_Ka <path>");
            }
            _currentMaterial.AmbientMap = values[0];
        }

        public void SetDiffuseMap(in ReadOnlySpan<string> values)
        {
            if (values.Length != 1)
            {
                throw new FormatException("Diffuse Map must be in format map_Kd <path>");
            }
            _currentMaterial.DiffuseMap = values[0];
        }

        public void SetAlphaMap(in ReadOnlySpan<string> values)
        {
            if (values.Length != 1)
            {
                throw new FormatException("Alpha Map must be in format map_d <path>");
            }
            _currentMaterial.AlphaMap = values[0];
        }

        public void SetBumpMap(in ReadOnlySpan<string> values)
        {
            if (values.Length != 1)
            {
                throw new FormatException("Bump Map must be in format map_bump <path>");
            }
            _currentMaterial.BumpMap = values[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ObjColor ParseColor(in ReadOnlySpan<string> values) => new(values, ParseFloat(values[0]), ParseFloat(values[1]), ParseFloat(values[2]));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ParseFloat(in string value) => float.Parse(value, CultureInfo.InvariantCulture);
    }
}
