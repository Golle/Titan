namespace Titan.ECS.SystemsV2;

public interface IDefault<out T>
{
    static abstract T Default();
}