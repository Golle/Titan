using System.Collections.Generic;

namespace Tools.ManifestBuilder.Parsers.WavefrontObj
{
    public class ObjGroup
    {
        public IList<ObjFace> Faces = new List<ObjFace>();
        public string Name { get; }
        public ObjGroup(string name = null)
        {
            Name = name;
        }

        public void AddFace(ObjFace face)
        {
            Faces.Add(face);
        }
    }
}
