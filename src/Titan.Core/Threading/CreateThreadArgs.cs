namespace Titan.Core.Threading;

public unsafe ref struct CreateThreadArgs
{
    public required delegate* unmanaged<void*, int> Callback;
    public void* Parameter;
    public bool StartImmediately;
}
