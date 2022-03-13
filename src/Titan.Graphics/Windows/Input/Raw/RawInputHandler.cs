using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Titan.Core.Logging;
using Titan.Windows;
using Titan.Windows.Win32;

namespace Titan.Graphics.Windows.Input.Raw;

internal unsafe class RawInputHandler
{
    private bool[] _buttonState = new bool[128];

    private int _xAxis, _yAxis, _zAxis, _rZAxis, _lHat;

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
        User32.GetRawInputDeviceInfoW(input->Header.hDevice, RIDICOMMAND.RIDI_PREPARSEDDATA, null, &size);

        void* pPreparsedData = NativeMemory.Alloc(size);
        var gotPreparsedData = User32.GetRawInputDeviceInfoW(input->Header.hDevice, RIDICOMMAND.RIDI_PREPARSEDDATA, pPreparsedData, &size) > 0;
        if (!gotPreparsedData)
        {
            NativeMemory.Free(pPreparsedData);
            return;
        }

        Unsafe.SkipInit(out HIDP_CAPS caps);
        if (Hid.HidP_GetCaps(pPreparsedData, &caps) != 0)
        {
            var pButtonCaps = (HIDP_BUTTON_CAPS*)NativeMemory.Alloc((nuint)(sizeof(HIDP_BUTTON_CAPS) * caps.NumberInputButtonCaps));
            
            var capsLength = caps.NumberInputButtonCaps;

            var hidPGetButtonCaps = Hid.HidP_GetButtonCaps(HIDP_REPORT_TYPE.HidP_Input, pButtonCaps, &capsLength, pPreparsedData);
            if (hidPGetButtonCaps == (int)HIDP_STATUS.HIDP_STATUS_SUCCESS)
            {

                var numberOfButtons = pButtonCaps->Range.UsageMax - pButtonCaps->Range.UsageMin + 1;

                var pValueCaps = (HIDP_VALUE_CAPS*)NativeMemory.Alloc((nuint)(sizeof(HIDP_VALUE_CAPS) * caps.NumberInputValueCaps));
                capsLength = caps.NumberInputValueCaps;
                if (Hid.HidP_GetValueCaps(HIDP_REPORT_TYPE.HidP_Input, pValueCaps, &capsLength, pPreparsedData) == (int)HIDP_STATUS.HIDP_STATUS_SUCCESS)
                {

                    {
                        var usage = stackalloc ushort[128];

                        var usageLength = (uint)numberOfButtons;
                        if (Hid.HidP_GetUsages(HIDP_REPORT_TYPE.HidP_Input, pButtonCaps->UsagePage, 0, usage, &usageLength, pPreparsedData, input->Hid.bRawData, input->Hid.dwSizeHid) == (int)HIDP_STATUS.HIDP_STATUS_SUCCESS)
                        {
                            Array.Fill(_buttonState, false);
                            for (var i = 0; i < usageLength; i++)
                            {
                                _buttonState[usage[i] - pButtonCaps->Range.UsageMin] = true;
                            }
                        }
                    }

                    for (var i = 0; i < caps.NumberInputValueCaps; i++)
                    {
                        uint value;
                        if (Hid.HidP_GetUsageValue(HIDP_REPORT_TYPE.HidP_Input, pValueCaps[i].UsagePage, 0, pValueCaps[i].Range.UsageMin, &value, pPreparsedData, input->Hid.bRawData, input->Hid.dwSizeHid) == (int)HIDP_STATUS.HIDP_STATUS_SUCCESS)
                        {
                            if (pValueCaps[i].IsRange == 0)
                            {
                                //Logger.Error("Crap, it's not correct..");
                                break;;
                            }
                            switch (pValueCaps[i].Range.UsageMin)
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

                NativeMemory.Free(pValueCaps);
            }
            else
            {
                Logger.Error($"{nameof(Hid.HidP_GetButtonCaps)} failed");
            }


            NativeMemory.Free(pButtonCaps);
        }
        else
        {
            Logger.Error($"{nameof(Hid.HidP_GetCaps)} failed");
        }


        //Logger.Error("Raw input");

        NativeMemory.Free(pPreparsedData);

        var b = new StringBuilder();
        b.Append($"{_xAxis.ToString().PadLeft(5)} {_yAxis.ToString().PadLeft(5)} {_zAxis.ToString().PadLeft(5)} {_rZAxis.ToString().PadLeft(5)} {_lHat.ToString().PadLeft(5)} - ");
        for (var i = 0; i < _buttonState.Length; ++i)
        {
            if (_buttonState[i])
            {
                b.Append($"{i} = Down, ");
            }
        }
        Logger.Info(b.ToString());
    }

    public void DeviceChange()
    {
        Logger.Error("Device change");
    }
}
