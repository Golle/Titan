using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Posix;

public static unsafe partial class LibWayland
{
    private const string DllName = "libwayland-client.so.0";

    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial wl_display* wl_display_connect(byte* name);


    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial void wl_display_disconnect(wl_display* display);


    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial int wl_display_dispatch(wl_display* display);


    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial int wl_event_loop_get_fd(wl_event_loop* loop);


    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial wl_registry* wl_display_get_registry(wl_display* display);


    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial int wl_display_roundtrip(wl_display* display);

    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial wl_proxy* wl_proxy_marshal_constructor(wl_proxy* proxy, uint opcode, wl_interface* pInterface);


    [LibraryImport(DllName)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    public static partial int wl_proxy_add_listener(wl_proxy* proxy, void* implementation, void* data);
    
    public static int wl_registry_add_listener(wl_registry* registry, wl_registry_listener* listener, void* data)
        => wl_proxy_add_listener((wl_proxy*)registry, listener, data);
}

public struct wl_display { }
public struct wl_proxy { }
public struct wl_event_loop { }
public struct wl_registry { }

public unsafe struct wl_message
{
    public byte* name;
    public byte* signature;
    public wl_interface** types;
}
public unsafe struct wl_interface
{
    public byte* name;
    public int version;
    public int request_count;
    public wl_message* requests;
    public int event_count;
    public wl_message* events;
}

public unsafe struct wl_registry_listener
{
    public delegate* unmanaged<void*, wl_registry*, uint, byte*, uint, void> Global;
    public delegate* unmanaged<void*, wl_registry*, uint, void> GlobalRemove;
}
