using System;
using System.Collections.Generic;
using System.Linq;
using Titan.IOC;

namespace Titan.ECS.Systems
{
    public class SystemsBuilder
    {
        private readonly List<Type> _systemTypes = new();
        public SystemsBuilder With<T>() where T : SystemBase
        {
            _systemTypes.Add(typeof(T));
            return this;
        }

        internal SystemBase[] Build(IContainer container)
        {
            Validate();

            return _systemTypes
                .Select(s => (SystemBase) container.CreateInstance(s))
                .ToArray();
        }

        private void Validate()
        {
            // do the stuffs
        }
    }
}
