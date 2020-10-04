using System;

namespace Titan.IOC
{
    internal class ContainerObject
    {
        public RegisteredObject RegisteredObject { get; }
        public object Instance { get; private set; }
        public ContainerObject(Type typeToResolve, Type concreteType, object instance = null)
        {
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
    }
}
