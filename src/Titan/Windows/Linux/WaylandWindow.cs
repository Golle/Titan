using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Titan.Core.Logging;
using Titan.Core.Maths;
using Titan.Platform.Posix;
using static Titan.Platform.Posix.LibWayland;

namespace Titan.Windows.Linux;

internal unsafe class WaylandWindow : IWindow
{
    public WindowHandle Handle { get; }
    public uint Width { get; }
    public uint Height { get; }
    public Size Size { get; }

    private wl_display* _display;
    private wl_registry* _registry;

    /*
     *pub static mut wl_registry_requests: [wl_message; 1] = [wl_message {
            name: b"bind\0" as *const u8 as *const c_char,
            signature: b"usun\0" as *const u8 as *const c_char,
            types: unsafe { &types_null as *const _ },
        }];
        #[doc = r" C-representation of the messages of this interface, for interop"]
        pub static mut wl_registry_events: [wl_message; 2] = [
            wl_message {
                name: b"global\0" as *const u8 as *const c_char,
                signature: b"usu\0" as *const u8 as *const c_char,
                types: unsafe { &types_null as *const _ },
            },
            wl_message {
                name: b"global_remove\0" as *const u8 as *const c_char,
                signature: b"u\0" as *const u8 as *const c_char,
                types: unsafe { &types_null as *const _ },
            },
        ];
     *
     */

    public bool Init(WindowCreationArgs args)
    {
        _display = wl_display_connect(null);
        if (_display == null)
        {
            Logger.Trace<WaylandWindow>("Failed to connect to the Wayland socket.");
            return false;
        }

        //NOTE(Jens): Memory leaks!
        //NOTE(Jens): This will set up the base structure of the Wayland client. We'll leave it like this for now.
        wl_message request;
        request.name = AllocCString("bind");
        request.signature = AllocCString("usun");
        request.types = null;

        var events = stackalloc wl_message[2];
        events[0].name = AllocCString("global");
        events[0].signature = AllocCString("usu");
        events[0].types = null;
        events[1].name = AllocCString("global_remove");
        events[1].signature = AllocCString("u");
        events[1].types = null;

        wl_interface getRegistry = default;
        getRegistry.name = AllocCString("wl_registry");
        getRegistry.version = 1;
        getRegistry.request_count = 1;
        getRegistry.requests = &request;
        getRegistry.event_count = 2;
        getRegistry.events = events;

        _registry = (wl_registry*)wl_proxy_marshal_constructor((wl_proxy*)_display, 1, &getRegistry);


        //_registry = wl_display_get_registry(_display);
        if (_registry == null)
        {
            Logger.Trace<WaylandWindow>("Failed to get registry.");
            return false;
        }


        wl_registry_listener listener;
        listener.Global = &registry_handle_global;
        listener.GlobalRemove = &registry_handle_global_remove;

        var res = wl_registry_add_listener(_registry, &listener, null);
        res = wl_display_dispatch(_display);
        res = wl_display_roundtrip(_display);


        return true;
    }

    private static byte* AllocCString(string value)
    {
        var bytes = Encoding.UTF8.GetByteCount(value) + 1;
        var buffer = (byte*)NativeMemory.Alloc((nuint)bytes);
        var length = Encoding.UTF8.GetBytes(value, new Span<byte>(buffer, bytes));
        buffer[length] = 0;
        return buffer;
    }
    public Point GetRelativeCursorPosition()
    {
        return default;
    }

    public void SetTitle(string title)
    {

    }

    public bool Update()
    {
        //return true;
        Debug.Assert(_display != null);
        var result = wl_display_dispatch(_display);
        return result != -1;
    }

    public void Shutdown()
    {
        Debug.Assert(_display != null);
        wl_display_disconnect(_display);
        _display = null;
    }


    [UnmanagedCallersOnly]
    private static void registry_handle_global(void* data, wl_registry* registry, uint id, byte* interfaceName, uint version)
    {

        var i = 0;
        while (interfaceName[i++] != 0) ;

        var str = Encoding.UTF8.GetString(new ReadOnlySpan<byte>(interfaceName, i - 1));
        
        Logger.Error<WaylandWindow>($"HEHE Global: {str}");
    }

    [UnmanagedCallersOnly]
    private static void registry_handle_global_remove(void* data, wl_registry* registry, uint id)
    {
        Logger.Error<WaylandWindow>("HEHE Remove");
    }

}
