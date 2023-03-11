namespace Titan.Platform.Win32.DBT;

public static class KSMediaTypes
{
    public static readonly Guid KSCATEGORY_AUDIO = new("6994AD04-93EF-11D0-A3CC-00A0C9223196");
}

public static class DEVICEINTERFACE_AUDIO
{
    public static readonly Guid DEVINTERFACE_AUDIO_RENDER = new("E6327CAD-DCEC-4949-AE8A-991E976A79D2");
    public static readonly Guid DEVINTERFACE_AUDIO_CAPTURE = new("2EEF81BE-33FA-4800-9670-1CD474972C3F");
}
