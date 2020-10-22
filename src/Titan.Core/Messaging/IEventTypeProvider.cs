using System;

namespace Titan.Core.Messaging
{
    public interface IEventTypeProvider
    {
        Type[] GetEventTypes();

    }
}
