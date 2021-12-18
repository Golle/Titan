namespace Titan.Core.Services;

public interface IServiceCollection
{
    public T Get<T>() where T : class;
}
