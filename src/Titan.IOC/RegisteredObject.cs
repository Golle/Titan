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

        private readonly ConstructorInfo _constructor;

        public RegisteredObject(Type typeToResolve, Type concreteType)
        {
            TypeToResolve = typeToResolve ?? throw new ArgumentNullException(nameof(typeToResolve));
            ConcreteType = concreteType ?? throw new ArgumentNullException(nameof(concreteType)); ;

            var constructors = ConcreteType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public); // include internal constructors
            if (constructors.Length == 0)
            {
                throw new NoConstructorDefinedException($"Type {ConcreteType.Name} is missing a public|internal constructor");
            }

            if (constructors.Length > 1)
            {
                throw new ToManyConstructorsException($"Type {ConcreteType.Name} have {constructors.Length} constructors defined. Only 1 is allowed.");
            }
            _constructor = constructors.First(); // TODO: replace with Expression.Lambda if needed for performance
            Parameters = _constructor.GetParameters();
        }

        public object CreateInstance(params object[] args) => _constructor.Invoke(args);
    }
}
