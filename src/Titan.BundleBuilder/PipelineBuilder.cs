using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Titan.BundleBuilder
{
    delegate Task<TContext> MiddlewareDelegate<TContext>(TContext context);
    internal sealed class PipelineBuilder<TContext>
    {
        private readonly List<Func<MiddlewareDelegate<TContext>, MiddlewareDelegate<TContext>>> _middlewares = new();

        public PipelineBuilder<TContext> Use(IMiddleware<TContext> middleware)
        {
            _middlewares.Add(next => async context =>
            {
                var name = middleware.GetType().Name;
                Logger.Info($"Executing {name}");
                return await middleware.Invoke(context, next);
            });
            return this;
        }
        public MiddlewareDelegate<TContext> Build()
        {
            MiddlewareDelegate<TContext> app = Task.FromResult;
            foreach (var middleware in _middlewares.AsReadOnly().Reverse())
            {
                app = middleware(app);
            }
            return app;
        }
    }
}
