namespace Titan.Core;

public interface IDefault<out T>
{
    static abstract T Default { get; }
}
