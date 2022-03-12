using System.ComponentModel;
using System.Runtime.InteropServices;
using Titan.Core.Logging;
using Titan.Windows;
using Titan.Windows.Win32;

namespace Titan.Graphics.Windows.Input.Raw;

internal unsafe class RawInputHandler
{
    public void Register(HWND hwnd)
    {
        // Register for input device on the current Window and for both generic joystick (ps4) and gamepad (xbox)
        var devices = stackalloc RAWINPUTDEVICE[2];
        devices[0] = new RAWINPUTDEVICE
        {
            usUsagePage = HID_USAGE_PAGE.HID_USAGE_PAGE_GENERIC,
            usUsage = HID_USAGE.HID_USAGE_GENERIC_GAMEPAD,
            hwndTarget = hwnd,
            dwFlags = (uint)RIDEV.RIDEV_DEVNOTIFY
        };
        devices[1] = devices[0];
        devices[1].usUsage = HID_USAGE.HID_USAGE_GENERIC_JOYSTICK;

        if (!User32.RegisterRawInputDevices(devices, 2, (uint)sizeof(RAWINPUTDEVICE)))
        {
            var error = Marshal.GetLastWin32Error();
            Logger.Error<RawInputHandler>($"Failed to rgister for raw input with code: 0x{error:x8}");
            throw new Win32Exception(error, "Failed to register raw input devices.");
        }

        Logger.Trace<RawInputHandler>("Successfully registered for RawInput");
    }

    public void Handle(nuint lParam, nuint wParam)
    {

        uint size;
        User32.GetRawInputData(lParam, RIDCOMMAND.RID_INPUT, null, &size, (uint)sizeof(RAWINPUTHEADER));
        if (size == 0)
        {
            return;
        }


        var input1 = stackalloc byte[(int)size];
        var input = (RAWINPUT*)input1;

        var receivedInput = User32.GetRawInputData(lParam, RIDCOMMAND.RID_INPUT, input, &size, (uint)sizeof(RAWINPUTHEADER)) > 0;
        if (!receivedInput)
        {
            return;
        }
        User32.GetRawInputDeviceInfoW(input->header.hDevice, RIDICOMMAND.RIDI_PREPARSEDDATA, null, &size);

        void* preparsedData = NativeMemory.Alloc(size);
        /// _HIDP_PREPARSED_DATA * data = (_HIDP_PREPARSED_DATA*)malloc(size);
        var gotPreparsedData = User32.GetRawInputDeviceInfoW(input->header.hDevice, RIDICOMMAND.RIDI_PREPARSEDDATA, preparsedData, &size) > 0;
        if (!gotPreparsedData)
        {
            NativeMemory.Free(preparsedData);
            return;
        }

        HIDP_CAPS caps;
        Hid.HidP_GetCaps(preparsedData, &caps);

        //Logger.Error("Raw input");

        NativeMemory.Free(preparsedData);
    }

    public void DeviceChange()
    {
        Logger.Error("Device change");
    }
}
