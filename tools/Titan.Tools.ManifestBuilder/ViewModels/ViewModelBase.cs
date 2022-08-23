using System;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class ViewModelBase : ReactiveObject
{
    public void SetProperty<T>(ref T reference, in T value, [CallerMemberName] string? propertyName = null)
        => this.RaiseAndSetIfChanged(ref reference, value, propertyName);

    public void SetProperty(Action action, [CallerMemberName] string? propertyName = null)
    {
        action();
        NotifyPropertyChanged(propertyName);
    }


    public void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        => this.RaisePropertyChanged(propertyName);
}

