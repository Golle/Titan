using Titan.Core.Logging;
using Titan.ECS.Systems;
using Titan.ECS.SystemsV2;
using Titan.Graphics.Modules;

namespace Titan.Input.Modules;

public struct MouseInputSystem : IStructSystem<MouseInputSystem>
{
    private ApiResource<WindowApi> _api;
    private ReadOnlyResource<Window> _window;

    public static void Init(ref MouseInputSystem system, in SystemsInitializer init)
    {
        system._api = init.GetApi<WindowApi>();
        system._window = init.GetReadOnlyResource<Window>();
    }

    public static void Update(ref MouseInputSystem system)
    {
        ref readonly var window = ref system._window.Get();
        if (system._api.Get().GetRelativeCursorPosition(window, out var p))
        {
            Logger.Info<MouseInputSystem>($"[{p.X}, {p.Y}]");
        }
    }

    public static bool ShouldRun(in MouseInputSystem system) => true;
}
