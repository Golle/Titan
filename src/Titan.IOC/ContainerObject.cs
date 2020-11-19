using System;

namespace Titan.IOC
{
    internal class ContainerObject : IDisposable
    {
        private readonly bool _dispose;
        public RegisteredObject RegisteredObject { get; }
        public object Instance { get; private set; }
        public ContainerObject(Type typeToResolve, Type concreteType, object instance = null, bool dispose = false)
        {
            _dispose = dispose;
            if (instance == null)
            {
                RegisteredObject = new RegisteredObject(typeToResolve, concreteType);
            }
            Instance = instance;
        }

        public object CreateInstance(params object[] args)
        {
            return Instance = RegisteredObject.CreateInstance(args) ?? throw new InvalidOperationException($"Failed to create instance of type {RegisteredObject.ConcreteType.Name} with interface {RegisteredObject.TypeToResolve.Name}");
        }

        public void Dispose()
        {
            if (_dispose && Instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
