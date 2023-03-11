namespace Titan.Windows.Events;

public readonly struct AudioDeviceRemovedEvent : IWindowEvent
{
    public static uint Id => WindowEventID.AudioDeviceRemoved;
}
