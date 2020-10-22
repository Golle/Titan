using System;
using System.Linq;

namespace Titan.Core.Messaging
{
    public class ScanningEventTypeProvider : IEventTypeProvider
    {
        public Type[] GetEventTypes() =>
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.GetCustomAttributes(false).Any(o => o.GetType() == typeof(TitanEventAttribute))))
                .ToArray();
    }
}
