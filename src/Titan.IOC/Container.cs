using System;
using System.Collections.Generic;
using System.Linq;
using Titan.IOC.Exceptions;

namespace Titan.IOC
{
    public class Container : IContainer
    {
        private readonly IContainer _parentContainer;
        private readonly IDictionary<Type, ContainerObject> _containerObjects = new Dictionary<Type, ContainerObject>();
        internal IEnumerable<RegisteredObject> RegisteredObjects => _containerObjects.Select(c => c.Value.RegisteredObject);

        public TTypeToResolve GetInstance<TTypeToResolve>()
        {
            return (TTypeToResolve)ResolveObject(typeof(TTypeToResolve));
        }

        public IContainer RegisterSingleton<TTypeToResolve>(TTypeToResolve instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            var typeToResolve = typeof(TTypeToResolve);
            if (_containerObjects.ContainsKey(typeToResolve))
            {
                throw new TypeAlreadyRegisteredException($"The type {typeToResolve.Name} has already been registered.");
            }
            lock (_containerObjects)
            {
                _containerObjects.Add(typeToResolve, new ContainerObject(typeToResolve, instance.GetType(), instance));
            }
            return this;
        }

        public object GetInstance(Type type)
        {
            return ResolveObject(type);
        }

        public Container(IContainer parentContainer = null)
        {
            _parentContainer = parentContainer;
        }

        public IContainer Register<TConcrete>() where TConcrete : class
        {
            return Register<TConcrete, TConcrete>();
        }

        public IContainer Register<TTypeToResolve, TConcrete>() where TConcrete : TTypeToResolve
        {
            var typeToResolve = typeof(TTypeToResolve);
            if (_containerObjects.ContainsKey(typeToResolve))
            {
                throw new TypeAlreadyRegisteredException($"The type {typeToResolve.Name} has already been registered.");
            }
            _containerObjects.Add(typeToResolve, new ContainerObject(typeToResolve, typeof(TConcrete)));
            return this;
        }

        public TTypeToResolve CreateInstance<TTypeToResolve>()
        {
            var typeToResolve = typeof(TTypeToResolve);
            return (TTypeToResolve)CreateInstance(typeToResolve);
        }

        public object CreateInstance(Type typeToResolve)
        {
            if (_containerObjects.TryGetValue(typeToResolve, out var containerObject))
            {
                return CreateInstance(containerObject);
            }

            // Create instance will not look for registered types in parent containers, but it will use the parent when resolving types for the constructor

            //var instance = _parentContainer?.CreateInstance(typeToResolve);
            //if (instance != null)
            //{
            //    return instance;
            //}

            // Try to create an instance of the type even if it hasn't been registered. Returns null if it fails.
            return CreateInstance(new ContainerObject(typeToResolve, typeToResolve));
        }

        private object GetOrCreateInstance(ContainerObject containerObject)
        {
            if (containerObject.Instance != null)
            {
                return containerObject.Instance;
            }
            lock (containerObject)
            {
                return containerObject.Instance ?? CreateInstance(containerObject);
            }
        }

        private object CreateInstance(ContainerObject containerObject)
        {
            var parameters = ResolveConstructorParameters(containerObject.RegisteredObject);
            var instance = containerObject.CreateInstance(parameters.ToArray());
            if (instance == null)
            {
                throw new InvalidOperationException($"Failed to create instance of type {containerObject.RegisteredObject.ConcreteType} with interface {containerObject.RegisteredObject.TypeToResolve}");
            }
            return instance;
        }

        private IEnumerable<object> ResolveConstructorParameters(RegisteredObject registeredObject)
        {
            return registeredObject
                .Parameters.Select(parameter => ResolveObject(parameter.ParameterType));
        }

        private object ResolveObject(Type typeToResolve)
        {
            if (!_containerObjects.ContainsKey(typeToResolve))
            {
                var instance = _parentContainer?.GetInstance(typeToResolve);
                if (instance != null)
                {
                    return instance;
                }
                throw new TypeNotRegisteredException($"The type {typeToResolve.Name} has not been registered.");
            }
            return GetOrCreateInstance(_containerObjects[typeToResolve]);
        }

        public IContainer AddRegistry<T>() where T : IRegistry
        {
            var registry = Activator.CreateInstance<T>();
            registry.Register(this);
            return this;
        }

        public IEnumerable<T> GetAll<T>()
        {
            var type = typeof(T);
            var parentObjects = _parentContainer?.GetAll<T>() ?? Enumerable.Empty<T>();
            return _containerObjects
                .Where(r => type.IsAssignableFrom(r.Key) || (r.Value.RegisteredObject != null && type.IsAssignableFrom(r.Value.RegisteredObject.ConcreteType)))
                .Select(r => GetOrCreateInstance(r.Value))
                .Cast<T>()
                .Concat(parentObjects);
        }

        public IContainer CreateChildContainer()
        {
            return new Container(this);
        }
    }
}
