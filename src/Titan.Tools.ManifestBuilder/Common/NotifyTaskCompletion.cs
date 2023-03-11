using System.ComponentModel;

namespace Titan.Tools.ManifestBuilder.Common;

public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
{
    public NotifyTaskCompletion(Task<TResult> task)
    {
        Task = task;
        if (!task.IsCompleted)
        {
            var _ = WatchTaskAsync(task);
        }
    }
    private async Task WatchTaskAsync(Task task)
    {
        try
        {
            await task;
        }
        catch
        {
            // ignored
        }

        var propertyChanged = PropertyChanged;
        if (propertyChanged == null)
            return;
        propertyChanged(this, new PropertyChangedEventArgs(nameof(Status)));
        propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
        propertyChanged(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));
        if (task.IsCanceled)
        {
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
        }
        else if (task.IsFaulted)
        {
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(Exception)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(InnerException)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
        }
        else
        {
            propertyChanged(this, new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
            propertyChanged(this, new PropertyChangedEventArgs(nameof(Result)));
        }
    }
    public Task<TResult> Task { get; }
    public TResult Result => Task.Status == TaskStatus.RanToCompletion ? Task.Result : default!;
    public TaskStatus Status => Task.Status;
    public bool IsCompleted => Task.IsCompleted;
    public bool IsNotCompleted => !Task.IsCompleted;
    public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;
    public bool IsCanceled => Task.IsCanceled;
    public bool IsFaulted => Task.IsFaulted;
    public AggregateException? Exception => Task.Exception;
    public Exception? InnerException => Exception?.InnerException;
    public string? ErrorMessage => InnerException?.Message;

    public event PropertyChangedEventHandler? PropertyChanged;
}
