namespace Titan.Core.Common
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(string json);
        string Serialize<T>(in T obj);
    }
}
