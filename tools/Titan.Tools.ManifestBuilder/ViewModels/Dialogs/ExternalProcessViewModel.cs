using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using DynamicData.Binding;

namespace Titan.Tools.ManifestBuilder.ViewModels.Dialogs;

public record ExternalProcess(string Filename, string Arguments, string? WorkingDir = null);

public class ExternalProcessViewModel : ViewModelBase
{
    private bool _isRunning;
    public bool IsRunning
    {
        get => _isRunning;
        set => SetProperty(ref _isRunning, value);
    }

    private string _status = "Running";
    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public IObservableCollection<string> LogOutput { get; } = new ObservableCollectionExtended<string>();


    private int _lineCount;
    public ExternalProcessViewModel(ExternalProcess externalProcess)
    {
        WriteLine($"Starting process {Path.GetFileNameWithoutExtension(externalProcess.Filename)} {externalProcess.Arguments}\n");

        var _ = Run(externalProcess);
    }

    private async Task Run(ExternalProcess externalProcess)
    {
        try
        {
            IsRunning = true;
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo(externalProcess.Filename, externalProcess.Arguments)
                {
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                },
            };
            process.OutputDataReceived += async (_, args) =>
            {
                if (args.Data != null)
                {
                    await WriteLineAsync(args.Data);
                }
            };

            process.ErrorDataReceived += async (_, args) =>
            {
                if (args.Data != null)
                {
                    await WriteLineAsync(args.Data);
                }
            };
            if (!process.Start())
            {
                Status = "Failed to start the process.";
                return;
            }
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            await process.WaitForExitAsync(CancellationToken.None); //NOTE(Jens): add support for cancellation token
            var exitCode = process.ExitCode;
            Status = $"Completed with exit code {exitCode}";
        }
        catch (Exception e)
        {
            Status = $"Exception occurred: {e.GetType().Name} - {e.Message}";
        }
        finally
        {
            IsRunning = false;
        }
    }
    public ExternalProcessViewModel()
    {
        for (var i = 0; i < 100; ++i)
            WriteLine("Design mode");
    }

    private async ValueTask WriteLineAsync(string line)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            WriteLine(line);
        }
        else
        {
            await Dispatcher.UIThread.InvokeAsync(() => WriteLine(line));
        }
    }

    private void WriteLine(string line)
        => LogOutput.Add($"{_lineCount++,4}: {line}");
}


