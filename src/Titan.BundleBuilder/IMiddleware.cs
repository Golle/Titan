using System.Threading.Tasks;

namespace Titan.BundleBuilder
{
    internal interface IMiddleware<TContext>
    {
        Task<TContext> Invoke(TContext context, MiddlewareDelegate<TContext> next);
    }
}
