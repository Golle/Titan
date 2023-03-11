namespace Titan.Setup;

public interface IRunner
{
    bool Init(IApp app);
    bool RunOnce();

    static abstract IRunner Create();
}
