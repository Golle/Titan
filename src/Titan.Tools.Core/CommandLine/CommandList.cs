using Titan.Core.Logging;

namespace Titan.Tools.Core.CommandLine;

public class CommandList<TResult> where TResult : class, new()
{
    private readonly string _executable;
    private readonly string? _title;
    private readonly List<ICommand> _commands = new();

    public CommandList(string executable, string? title = null)
    {
        _executable = executable;
        _title = title;
    }
    public CommandList<TResult> WithCommand(ICommand command)
    {
        if (_commands.Any(c => c.Name == command.Name))
        {
            throw new InvalidOperationException($"Multiple definitions of command {command.Name}");
        }
        _commands.Add(command);
        return this;
    }

    public async Task<TResult?> Execute(string[] args)
    {
        if (args.Length == 0 || args.Contains("--help"))
        {
            Print();
            return new TResult();
        }

        var command = _commands.SingleOrDefault(c => c.Name.Equals(args[0], StringComparison.OrdinalIgnoreCase));
        if (command == null)
        {
            Logger.Error($"Command {args[0]} does not exist.");
            return null;
        }

        return await command.Execute<TResult>(args.Skip(1).ToArray());
    }

    public void Print()
    {
        Logger.Raw($"Usage: {_executable} [command] [options]\n");
        if (_title != null)
        {
            Logger.Raw(_title);
        }

        Logger.Raw("Command: help");
        Logger.Raw($"Usage:\n {_executable} --help\n");

        foreach (var command in _commands)
        {
            command.Print(_executable);
        }
    }
}
