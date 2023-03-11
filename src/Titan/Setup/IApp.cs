using Titan.Core;
using Titan.Setup.Configs;

namespace Titan.Setup;

public interface IApp
{
    T GetConfigOrDefault<T>() where T : IConfiguration, IDefault<T>;
    T GetConfig<T>() where T : IConfiguration;
    IEnumerable<T> GetConfigs<T>() where T : IConfiguration;
    ref T GetResource<T>() where T : unmanaged;
    unsafe T* GetResourcePointer<T>() where T : unmanaged;
    T GetManagedResource<T>() where T : class;
    ObjectHandle<T> GetManagedResourceHandle<T>() where T : class;
    void Run();
}
