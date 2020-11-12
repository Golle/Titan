namespace Titan.AssetConverter.WavefrontObj
{
    public class ObjMaterial
    {
        public string Name { get; }
        public ObjColor AmbientColor { get; set; } // Ka
        public ObjColor DiffuseColor { get; set; } // Kd
        public ObjColor SpecularColor { get; set; } // Ks
        public ObjColor EmissiveColor { get; set; } // Ke
        public float Alpha { get; set; } = 1f; // d
        public float Transparency { get; set; } // Tr
        public float Shininess { get; set; } // Ns
        public float Illumination { get; set; } // illum
        public string AmbientMap { get; set; } // map_Ka
        public string DiffuseMap { get; set; } // map_Kd
        public string AlphaMap { get; set; } // map_d
        public string BumpMap { get; set; } // map_bump


        public ObjMaterial(in string name)
        {
            Name = name;
        }
    }
}