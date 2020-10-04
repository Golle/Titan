using System;
using System.Linq;
using System.Reflection;
using Titan.IOC.Exceptions;

namespace Titan.IOC
{
    internal class RegisteredObject
    {
        public Type TypeToResolve { get; }
        public Type ConcreteType { get; }
        public ParameterInfo[] Parameters { get; }

        public RegisteredObject(Type typeToResolve, Type concreteType)
        {
            TypeToResolve = typeToResolve ?? throw new ArgumentNullException(nameof(typeToResolve));
            ConcreteType = concreteType ?? throw new ArgumentNullException(nameof(concreteType)); ;
         
            var constructors = ConcreteType.GetConstructors();
            if (constructors.Length == 0)
            {
                throw new NoConstructorDefinedException($"Type {ConcreteType.Name} is missing a constructor");
            }

            if (constructors.Length > 1)
            {
                throw new ToManyConstructorsException($"Type {ConcreteType.Name} have {constructors.Length} constructors defined. Only 1 is allowed.");
            }
            Parameters = constructors.First().GetParameters();
        }

        public object CreateInstance(params object[] args)
        {
            return Activator.CreateInstance(ConcreteType, args);
        }
    }
}
