using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core.Messaging;
using Titan.ECS.Entities;

namespace Titan.Events;

[StructLayout(LayoutKind.Sequential)]
[SkipLocalsInit]
public readonly struct BoxCollisionEnterEvent
{
    public static readonly short Id = EventId<BoxCollisionEnterEvent>.Value;
    public readonly Entity Entity;
    public readonly Entity Target;
    public readonly uint EntityMask;
    public readonly uint TargetMask;
    
    public BoxCollisionEnterEvent(in Entity entity, in Entity target, uint entityMask, uint targetMask)
    {
        Entity = entity;
        Target = target;
        EntityMask = entityMask;
        TargetMask = targetMask;
    }
}
