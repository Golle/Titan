using System;
using Splat;
using Titan.Tools.ManifestBuilder.Services;

namespace Titan.Tools.ManifestBuilder;

internal static class Registry
{
    public static void Init(IMutableDependencyResolver services) =>
        services
            .RegisterLazySingleton<IJsonSerializer>(_ => new JsonSerializer())
            .RegisterLazySingleton<IAppConfiguration>(_ => new AppConfiguration())
            .RegisterLazySingleton<IManifestService>(resolver => new ManifestService(resolver.GetRequiredService<IAppConfiguration>(), resolver.GetRequiredService<IJsonSerializer>()))

    //services
    //.RegisterLazySingleton<IModalService>(_ => new ModalService())
    ;



    //NOTE(Jens): Some helper methods for this DI framework from the 80s.
    public static T GetRequiredService<T>(this IReadonlyDependencyResolver resolver)
    {
        if (resolver is null)
        {
            throw new ArgumentNullException(nameof(resolver));
        }

        var service = resolver.GetService(typeof(T));
        if (service == null)
        {
            throw new InvalidOperationException($"Service {typeof(T).Name} has not been registered.");
        }
        return (T)service;
    }

    public static IMutableDependencyResolver RegisterLazySingleton<T>(this IMutableDependencyResolver services, Func<IReadonlyDependencyResolver, object> factory)
    {
        services.RegisterLazySingleton(() => factory((IReadonlyDependencyResolver)services), typeof(T));
        return services;
    }
}
