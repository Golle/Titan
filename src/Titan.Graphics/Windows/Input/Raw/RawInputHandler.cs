using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using Titan.Core.Logging;
using Titan.Windows;
using Titan.Windows.Win32;

namespace Titan.Graphics.Windows.Input.Raw;


internal unsafe struct RawInputDevice
{
    public HID_USAGE_PAGE UsagePage;
    public HID_USAGE Usage;

    private fixed byte _buttonCaps[1];
}

internal unsafe class RawInputHandler
{
    public const int MaxButtons = 128;

    private const int MaxDevices = 16;


    private readonly bool[] _buttonState = new bool[MaxButtons];

    private int _xAxis, _yAxis, _zAxis, _rZAxis, _lHat;


    private void* _preparsedData = NativeMemory.Alloc(1024 * 1024);
    private HIDP_BUTTON_CAPS* _buttonCaps = (HIDP_BUTTON_CAPS*)NativeMemory.Alloc(1024 * 1024);
    private HIDP_VALUE_CAPS* _valueCaps = (HIDP_VALUE_CAPS*)NativeMemory.Alloc(1024 * 1024);


    private HWND _hwnd;
    public void Register(HWND hwnd)
    {
        // Register for input device on the current Window and for both generic joystick (ps4) and gamepad (xbox)
        var devices = stackalloc RAWINPUTDEVICE[2];
        devices[0] = new RAWINPUTDEVICE
        {
            usUsagePage = HID_USAGE_PAGE.HID_USAGE_PAGE_GENERIC,
            usUsage = HID_USAGE.HID_USAGE_GENERIC_GAMEPAD,
            hwndTarget = _hwnd,
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
        uint size = 500;
        //User32.GetRawInputData(lParam, RIDCOMMAND.RID_INPUT, null, &size, (uint)sizeof(RAWINPUTHEADER));
        //timer.Stop();
        //_ticks[0] = timer.Elapsed.Ticks;
        //if (size == 0)
        //{
        //    return null;
        //}

        var input1 = stackalloc byte[(int)size];
        var input = (RAWINPUT*)input1;

        var receivedInput = User32.GetRawInputData(lParam, RIDCOMMAND.RID_INPUT, input, &size, (uint)sizeof(RAWINPUTHEADER)) > 0;
        if (!receivedInput)
        {
            return;
        }
        size = 1024 * 1024;
        var gotPreparsedData = User32.GetRawInputDeviceInfoA(input->Header.hDevice, RIDICOMMAND.RIDI_PREPARSEDDATA, _preparsedData, &size) > 0;
        if (!gotPreparsedData)
        {
            return;
        }

        Unsafe.SkipInit(out HIDP_CAPS caps);

        if (Hid.HidP_GetCaps(_preparsedData, &caps) != 0)
        {
            var capsLength = caps.NumberInputButtonCaps;

            var hidPGetButtonCaps = Hid.HidP_GetButtonCaps(HIDP_REPORT_TYPE.HidP_Input, _buttonCaps, &capsLength, _preparsedData);
            if (hidPGetButtonCaps == (int)HIDP_STATUS.HIDP_STATUS_SUCCESS)
            {

                var numberOfButtons = _buttonCaps->Range.UsageMax - _buttonCaps->Range.UsageMin + 1;

                capsLength = caps.NumberInputValueCaps;
                if (Hid.HidP_GetValueCaps(HIDP_REPORT_TYPE.HidP_Input, _valueCaps, &capsLength, _preparsedData) == (int)HIDP_STATUS.HIDP_STATUS_SUCCESS)
                {
                    {
                        var usage = stackalloc ushort[128];

                        var usageLength = (uint)numberOfButtons;
                        if (Hid.HidP_GetUsages(HIDP_REPORT_TYPE.HidP_Input, _buttonCaps->UsagePage, 0, usage, &usageLength, _preparsedData, input->Hid.bRawData, input->Hid.dwSizeHid) == (int)HIDP_STATUS.HIDP_STATUS_SUCCESS)
                        {
                            Array.Fill(_buttonState, false);
                            for (var i = 0; i < usageLength; i++)
                            {
                                _buttonState[usage[i] - _buttonCaps->Range.UsageMin] = true;
                            }
                        }
                    }
                    for (var i = 0; i < caps.NumberInputValueCaps; i++)
                    {
                        uint value;

                        if (Hid.HidP_GetUsageValue(HIDP_REPORT_TYPE.HidP_Input, _valueCaps[i].UsagePage, 0, _valueCaps[i].Range.UsageMin, &value, _preparsedData, input->Hid.bRawData, input->Hid.dwSizeHid) == (int)HIDP_STATUS.HIDP_STATUS_SUCCESS)

                        {
                            switch (_valueCaps[i].Range.UsageMin)
                            {
                                case 0x30: // X-axis
                                    _xAxis = (int)value - 128;
                                    break;

                                case 0x31: // Y-axis
                                    _yAxis = (int)value - 128;
                                    break;

                                case 0x32: // Z-axis
                                    _zAxis = (int)value - 128;
                                    break;

                                case 0x35: // Rotate-Z
                                    _rZAxis = (int)value - 128;
                                    break;

                                case 0x39: // Hat Switch
                                    _lHat = (int)value;
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                Logger.Error($"{nameof(Hid.HidP_GetButtonCaps)} failed");
            }
        }
        else
        {
            Logger.Error($"{nameof(Hid.HidP_GetCaps)} failed");
        }
        //var b = new StringBuilder();
        //b.Append($"{_xAxis.ToString().PadLeft(5)} {_yAxis.ToString().PadLeft(5)} {_zAxis.ToString().PadLeft(5)} {_rZAxis.ToString().PadLeft(5)} {_lHat.ToString().PadLeft(5)} - ");
        //for (var i = 0; i < _buttonState.Length; ++i)
        //{
        //    if (_buttonState[i])
        //    {
        //        b.Append($"{i} = Down, ");
        //    }
        //}
        //Logger.Info(b.ToString());


    }

    public void DeviceChange(nuint lParam, WPARAM wParam)
    {
        const nuint Arrival = 1;
        const nuint Removed = 2;


        uint size;
        var a = User32.GetRawInputData(lParam, RIDCOMMAND.RID_INPUT, null, &size, (uint)sizeof(RAWINPUTHEADER));

        switch (wParam.Value)
        {
            case Arrival:
                DeviceArrival(lParam);
                //_loadedDevices[0] = lParam;
                break;
            case Removed: DeviceRemoved(lParam); break;
            default:
                Logger.Warning<RawInputHandler>($"Recieved unrecognized command {wParam} for device change event.");
                break;
        }


        static void DeviceArrival(nuint handle)
        {
            var size = 1024;
            var buff = stackalloc char[size];
            if (User32.GetRawInputDeviceInfoW(handle, RIDICOMMAND.RIDI_DEVICENAME, buff, (uint*)&size) > 0)
            {
                var name = new string(buff, 0, size);
                Logger.Info<RawInputHandler>($"Device with Handle {handle} connected ({name})");
            }

            RID_DEVICE_INFO info;
            size = sizeof(RID_DEVICE_INFO);
            if (User32.GetRawInputDeviceInfoW(handle, RIDICOMMAND.RIDI_DEVICEINFO, &info, (uint*)&size) > 0)
            {
                ref readonly var hid = ref info.Hid;
                Logger.Info<RawInputHandler>($"Device info: {hid.usUsage} {hid.usUsagePage} 0x{hid.dwProductId.Value:X} 0x{hid.dwVendorId.Value:X} {hid.dwVersionNumber}");
                Logger.Info<RawInputHandler>($"Vendor: {DeviceTranslation.VendorName(hid.dwVendorId)} Product: {DeviceTranslation.ProductName(hid.dwProductId)}");
            }


        }

        static void DeviceRemoved(nuint handle)
        {
            Logger.Info<RawInputHandler>($"Device with Handle {handle} removed");
        }
    }

    public uint BufferedRead()
    {
        var size = 5 * 1024;
        var rawInput = stackalloc byte[size];
        var usage = stackalloc ushort[128];

        var count = User32.GetRawInputBuffer((RAWINPUT*)rawInput, (uint*)&size, (uint)sizeof(RAWINPUTHEADER));
        Unsafe.SkipInit(out HIDP_CAPS caps);

        //var builder = new StringBuilder();
        var offset = 0u;
        for (var aaa = 0; aaa < count; ++aaa)
        {
            var raw = (RAWINPUT*)(rawInput + offset);
            var dataSize = 1024u * 1024u;
            //var gotPreparsedData = User32.GetRawInputDeviceInfoA(raw->Header.hDevice, RIDICOMMAND.RIDI_PREPARSEDDATA, _preparsedData, &dataSize) > 0;
            var gotPreparsedData = User32.GetRawInputDeviceInfoA(raw->Header.hDevice, RIDICOMMAND.RIDI_PREPARSEDDATA, _preparsedData, &dataSize) > 0;
            if (!gotPreparsedData)
            {
                continue;
            }


            if (Hid.HidP_GetCaps(_preparsedData, &caps) != 0)
            {
                var capsLength = caps.NumberInputButtonCaps;

                var hidPGetButtonCaps = Hid.HidP_GetButtonCaps(HIDP_REPORT_TYPE.HidP_Input, _buttonCaps, &capsLength, _preparsedData);
                if (hidPGetButtonCaps == (int)HIDP_STATUS.HIDP_STATUS_SUCCESS)
                {

                    var numberOfButtons = _buttonCaps->Range.UsageMax - _buttonCaps->Range.UsageMin + 1;

                    capsLength = caps.NumberInputValueCaps;
                    if (Hid.HidP_GetValueCaps(HIDP_REPORT_TYPE.HidP_Input, _valueCaps, &capsLength, _preparsedData) == (int)HIDP_STATUS.HIDP_STATUS_SUCCESS)
                    {
                        {
                            var usageLength = (uint)numberOfButtons;
                            if (Hid.HidP_GetUsages(HIDP_REPORT_TYPE.HidP_Input, _buttonCaps->UsagePage, 0, usage, &usageLength, _preparsedData, raw->Hid.bRawData, raw->Hid.dwSizeHid) == (int)HIDP_STATUS.HIDP_STATUS_SUCCESS)
                            {
                                Array.Fill(_buttonState, false);
                                for (var i = 0; i < usageLength; i++)
                                {
                                    _buttonState[usage[i] - _buttonCaps->Range.UsageMin] = true;
                                }
                            }
                        }
                        for (var i = 0; i < caps.NumberInputValueCaps; i++)
                        {
                            uint value;

                            if (Hid.HidP_GetUsageValue(HIDP_REPORT_TYPE.HidP_Input, _valueCaps[i].UsagePage, 0, _valueCaps[i].Range.UsageMin, &value, _preparsedData, raw->Hid.bRawData, raw->Hid.dwSizeHid) == (int)HIDP_STATUS.HIDP_STATUS_SUCCESS)
                            {
                                switch (_valueCaps[i].Range.UsageMin)
                                {
                                    case 0x30: // X-axis
                                        _xAxis = (int)value - 128;
                                        break;

                                    case 0x31: // Y-axis
                                        _yAxis = (int)value - 128;
                                        break;

                                    case 0x32: // Z-axis
                                        _zAxis = (int)value - 128;
                                        break;

                                    case 0x35: // Rotate-Z
                                        _rZAxis = (int)value - 128;
                                        break;

                                    case 0x39: // Hat Switch
                                        _lHat = (int)value;
                                        break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Logger.Error($"{nameof(Hid.HidP_GetButtonCaps)} failed");
                }
            }
            else
            {
                Logger.Error($"{nameof(Hid.HidP_GetCaps)} failed");
            }
            offset += raw->Header.dwSize.Value;
        }


        return offset;
        //Logger.Error(builder.ToString());


    }
}
