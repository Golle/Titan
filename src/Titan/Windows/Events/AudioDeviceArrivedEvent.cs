namespace Titan.Windows.Events;

public readonly struct AudioDeviceArrivedEvent : IWindowEvent
{
    public static uint Id => WindowEventID.AudioDeviceArrived;
}
