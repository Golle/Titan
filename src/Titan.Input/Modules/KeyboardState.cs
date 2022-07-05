using Titan.Core;

namespace Titan.Input.Modules;

public unsafe struct KeyboardState : IResource
{
    public fixed bool Current[(int)KeyCode.NumberOfKeys];
    public fixed bool Previous[(int)KeyCode.NumberOfKeys];
}
