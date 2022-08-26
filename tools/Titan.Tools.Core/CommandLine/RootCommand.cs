using Titan.Core.Logging;

namespace Titan.Tools.Core.CommandLine;

public class RootCommand<T> : ICommand where T : class, new()
{
    public string Name { get; }
    private readonly HashSet<Option<T>> _options = new();
    private T _state;
    private Func<T, Task<T>> _execute;

    public RootCommand(string name)
    {
        Name = name;
        _state = new T();
        _execute = _ => Task.FromResult(_state);
    }

    public RootCommand<T> WithOption(Option<T> option)
    {
        if (!_options.Add(option))
        {
            throw new InvalidOperationException($"Multiple definitions for option {option.Name}");
        }
        return this;
    }

    public RootCommand<T> OnCommand(Func<T, Task<T>> run)
    {
        _execute = run;
        return this;
    }

    public async Task<TResult?> Execute<TResult>(string[] values) where TResult : class
    {
        Logger.Trace($"Executing command {typeof(T).Name}");
        for (var i = 0; i < values.Length; ++i)
        {
            var value = values[i];

            if (!IsOption(value))
            {
                Logger.Error($"{value} is not an option. Options must start with - or --.");
                return null;
            }
            var name = value.TrimStart('-');
            var option = _options.SingleOrDefault(o => name.Equals(o.Name) || name.Equals(o.Alias));
            if (option == null)
            {
                Logger.Error($"Unrecognized option {value}");
                return null;
            }

            string? arg = null;
            if (option.RequiresArguments)
            {
                if (i == values.Length)
                {
                    Logger.Error($"Option {option.Name} expects an argument.");
                    return null;
                }
                arg = values[++i];
                if (!option.Validate?.Invoke(arg) ?? true)
                {
                    Logger.Error($"Validation of argument '{arg}' failed for option {option.Name}");
                    return null;
                }

            }
            _state = option.Callback?.Invoke(_state, arg) ?? throw new InvalidOperationException("Callback method can not return null.");
        }

        var result = await _execute(_state);
        if (result is TResult res)
        {
            return res;
        }

        throw new InvalidOperationException($"{typeof(T)} is not extending/implementing {typeof(TResult)}.");

        static bool IsOption(string value) => value.StartsWith("--") || value.StartsWith("-");
    }

    public void Print(string executable)
    {
        const int OptionsWidth = 25;
        Logger.Raw($"Command: {Name}");
        Logger.Raw($"Usage:\n {executable} {Name} [options]\n");
        Logger.Raw("Options:");
        foreach (var option in _options)
        {
            var str = string.Empty;
            if (option.Alias != null)
            {
                str += $" -{option.Alias}, --{option.Name}".PadRight(OptionsWidth);
            }
            else
            {
                str += $" --{option.Name}".PadRight(OptionsWidth);
            }

            Logger.Raw(str + option.Description);
        }
        Logger.Raw("");
    }
}
