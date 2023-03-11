namespace Titan.Setup;

public interface IDefault<out T>
{
    static abstract T Default { get; }
}
