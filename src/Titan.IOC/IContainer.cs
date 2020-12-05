using System;
using System.Collections.Generic;

namespace Titan.IOC
{
    public interface IContainer : IDisposable
    {
        IContainer Register<TConcrete>(bool dispose = false) where TConcrete : class;
        IContainer Register<TTypeToResolve, TConcrete>(bool dispose = false) where TConcrete : TTypeToResolve;
        TTypeToResolve CreateInstance<TTypeToResolve>();
        object CreateInstance(Type type);
        TTypeToResolve GetInstance<TTypeToResolve>();
        IContainer RegisterSingleton<TTypeToResolve>(TTypeToResolve instance);
        object GetInstance(Type type);
        IContainer AddRegistry<T>() where T : IRegistry;
        IEnumerable<T> GetAll<T>();
        IContainer CreateChildContainer();
    }
}
