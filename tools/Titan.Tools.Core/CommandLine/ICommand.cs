namespace Titan.Tools.Core.CommandLine;

public interface ICommand
{
    string Name { get; }
    void Print(string executable);
    Task<TResult?> Execute<TResult>(string[] values) where TResult : class;
}