using System.Collections.Generic;

namespace Titan.Graphics.Windows.Input.Raw;

public static class DeviceTranslation
{
    // NOTE(Jens): these IDs were found at http://www.linux-usb.org/usb.ids
    private static readonly Dictionary<uint, string> _vendorNames = new()
    {
        { 0x045e, "Microsoft Corp." },
        { 0x054c, "Sony Corp." }
    };

    private static readonly Dictionary<uint, string> _productNames = new()
    {
        // Playstation
        { 0x05c4, "DualShock 4[CUH - ZCT1x]" },
        { 0x09cc, "DualShock 4[CUH - ZCT2x]" },
        { 0x0ba0, "Dualshock4 Wireless Adaptor" },
        { 0x0ce6, "DualSense wireless controller(PS5)" },

        // Xbox
        { 0x0202, "Xbox Controller" },
        { 0x0285, "Xbox Controller S" },
        { 0x0288, "Xbox Controller S Hub" },
        { 0x0289, "Xbox Controller S" },
        { 0x028e, "Xbox360 Controller" },
        { 0x028f, "Xbox360 Wireless Controller" },
        { 0x0291, "Xbox 360 Wireless Receiver for Windows" },
        { 0x02a0, "Xbox360 Big Button IR" },
        { 0x02a1, "Xbox 360 Wireless Receiver for Windows" },
        { 0x02d1, "Xbox One Controller" },
        { 0x02dd, "Xbox One Controller (Firmware 2015)" },
        { 0x02e3, "Xbox One Elite Controller" },
        { 0x02e6, "Wireless XBox Controller Dongle" },
        { 0x02ea, "Xbox One S Controller" },
        { 0x02fd, "Xbox One S Controller [Bluetooth]" },
    };

    public static string VendorName(uint vendorId) => _vendorNames.TryGetValue(vendorId, out var name) ? name : string.Empty;
    public static string ProductName(uint productId) => _productNames.TryGetValue(productId, out var name) ? name : string.Empty;
}
