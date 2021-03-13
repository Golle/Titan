using System.Threading.Tasks;
using Titan.AssetConverter.Exporter;

namespace Titan.AssetConverter.Pipeline.Middlewares
{
    internal class ObjConverterMiddleware  : IMiddleware<MeshContext>
    {
        public async Task<MeshContext> Invoke(MeshContext context, MiddlewareDelegate<MeshContext> next)
        {
            var builder = new MeshBuilder();
            foreach (var objGroup in context.Object.Groups)
            {
                foreach (var face in objGroup.Faces)
                {
                    // A new material indicates a new "sub mesh". This is where we create a new mesh. (if it's the first one we don't do anything except setting the material
                    builder.SetMaterial(face.Material);
                    
                    ////// TODO: add support for Concave faces (triangles done this way might overlap)
                    ////// RH
                    ////// 1st face => vertex 0, 1, 2
                    ////// 2nd face => vertex 0, 2, 3
                    ////// 3rd face => vertex 0, 4, 5
                    ////// 4th face => ...
                    var faceVertices = face.Vertices;
                    //// more than 3 vertices per face, we need to triangulate the face to be able to use it in the engine.
                    for (var i = 2; i < faceVertices.Length; ++i)
                    {
                        builder.AddVertex(faceVertices[0]);
                        //Flip the order to convert from RH to LH (d3d) 
                        builder.AddVertex(faceVertices[i]);
                        builder.AddVertex(faceVertices[i - 1]);
                    }
                }
            }

            var mesh = builder.Build(new VertexMapper(context.Object));
            return await next(context with {Mesh = mesh});
        }
    }
}