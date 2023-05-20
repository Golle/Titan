using System.Diagnostics;
using System.Text;

namespace Titan.Tools.Editor.Core;

internal class ProcessRunner : IProcessRunner
{
    public async Task<ProcessResult> Run(ProcessArgs args)
    {
        //Logger.Trace($"Run command: {command} {arguments}");
        //Logger.Trace($"Working directory: {workingDirectory}");
        var startInfo = new ProcessStartInfo(args.Command, args.Arguments)
        {
            WorkingDirectory = args.WorkgingDiretory,
            RedirectStandardOutput = !args.CreateWindow,
            RedirectStandardError = !args.CreateWindow,
            CreateNoWindow = !args.CreateWindow
        };

        Process process;
        try
        {
            process = new Process { StartInfo = startInfo };
            if (!process.Start())
            {
                return new ProcessResult
                {
                    Success = false,
                    Reason = $"Failed to start command {args.Command}"
                };
            }
        }
        catch (Exception e)
        {
            return new ProcessResult
            {
                Success = false,
                Reason = $"Failed because of {e.GetType().Name} - {e.Message}"
            };
        }

        var stdout = new StringBuilder();
        var stderr = new StringBuilder();
        if (!args.CreateWindow)
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.OutputDataReceived += (_, outputArgs) =>
            {
                if (outputArgs.Data != null)
                {
                    stdout.AppendLine(outputArgs.Data);
                }
            };
            process.ErrorDataReceived += (_, errorArgs) =>
            {
                if (errorArgs.Data != null)
                {
                    stdout.AppendLine(errorArgs.Data);
                }
            };
        }

        CancellationTokenSource timeoutSource = new();
        var token = CancellationToken.None;
        if (args.Timeout != TimeSpan.MaxValue)
        {
            timeoutSource.CancelAfter(args.Timeout);
            token = timeoutSource.Token;
        }
        try
        {

            await process.WaitForExitAsync(token);
        }
        catch (Exception e) when (e is OperationCanceledException or TaskCanceledException)
        {
            return new ProcessResult
            {
                Success = false,
                Reason = $"Timeout ({args.Timeout}) reached for command {args.Command} ({e.GetType().Name})",
                StdErr = stderr.ToString(),
                StdOut = stdout.ToString()
            };
        }
        finally
        {
            timeoutSource.Dispose();
        }

        if (!process.HasExited)
        {
            process.Kill(true);
        }
        return new ProcessResult
        {
            ExitCode = process.ExitCode,
            Success = process.ExitCode == 0,
            StdErr = stderr.ToString(),
            StdOut = stdout.ToString()
        };
    }

    public async Task<ProcessResult> RunNoWait(ProcessArgs args)
    {
        var startInfo = new ProcessStartInfo(args.Command, args.Arguments)
        {
            WorkingDirectory = args.WorkgingDiretory,
        };

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            return new ProcessResult
            {
                Reason = "Failed to start the process",
                Success = false
            };
        }
        await Task.Delay(TimeSpan.FromSeconds(2));
        if (process.HasExited && args.CheckForImmediateExit)
        {
            return new ProcessResult
            {
                Reason = "Process exited immediately",
                Success = false
            };
        }
        return new ProcessResult
        {
            Success = true
        };
    }
}
