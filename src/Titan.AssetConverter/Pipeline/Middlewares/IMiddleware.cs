using System.Threading.Tasks;

namespace Titan.AssetConverter.Pipeline.Middlewares
{
    internal interface IMiddleware<TContext>
    {
        Task<TContext> Invoke(TContext context, MiddlewareDelegate<TContext> next);
    }
}
