using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.Graphics.Modules;

namespace Titan.Graphics.D3D12;

public struct D3D12RenderModule : IModule
{
    public static void Build(AppBuilder builder)
    {
        ref readonly var window = ref builder.GetResource<Window>();
        if (D3D12Device.CreateAndInit(window.Handle, window.Width, window.Height, true, out var device))
        {
            Logger.Info<D3D12RenderModule>($"Created the {nameof(D3D12Device)}!");
            device.Release();
        }
        else
        {
            Logger.Error<D3D12RenderModule>($"Failed to create {nameof(D3D12Device)} :(");
        }


    }
}
