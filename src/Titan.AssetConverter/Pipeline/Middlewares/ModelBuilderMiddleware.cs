using System.Threading.Tasks;

namespace Titan.AssetConverter.Pipeline.Middlewares
{
    internal class ModelBuilderMiddleware : IMiddleware<MeshContext>
    {
        public Task<MeshContext> Invoke(MeshContext context, MiddlewareDelegate<MeshContext> next)
        {
            //var indices = context.Indices.Span;

            //var filename = Path.GetFileNameWithoutExtension(context.Filename);
            //var count = 0;
            //List<Mesh> meshes = new(context.Meshes.Length);
            //foreach (ref readonly var submesh in context.Meshes.Span)
            //{
            //    var name = $"{filename}.{count.ToString().PadLeft(3, '0')}";
            //    for (var i = 0; i < submesh.Count; ++i)
            //    {

            //    }



            //    //var indices = context.Indices
                


            //    count++;
            //}

            return next(context);



        }
    }

}
