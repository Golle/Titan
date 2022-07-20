using Titan.Core;

namespace Titan.Graphics.Modules;

public interface IWindowEvent : IEvent
{
    static abstract uint Id { get; }
}
