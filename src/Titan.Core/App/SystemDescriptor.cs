using System;

namespace Titan.Core.App;

public readonly struct SystemDescriptor
{
    public readonly Stage Stage;
    public readonly Func<ISystem> Creator;
    public SystemDescriptor(Func<ISystem> creator, Stage stage = Stage.Update)
    {
        Stage = stage;
        Creator = creator;
    }
}
