﻿namespace Titan.Platform.Win32.XAudio2;

public struct XAUDIO2_VOICE_SENDS
{
    public uint SendCount;                   // Number of sends from this voice.
    public unsafe XAUDIO2_SEND_DESCRIPTOR* pSends;    // Array of SendCount send descriptors.
}
