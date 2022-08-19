using System;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using Titan.Tools.ManifestBuilder.Common;
using Titan.Tools.ManifestBuilder.Services;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public record struct ProjectSelectedMessage(string ProjectPath);

public class ProjectSelectorViewModel : ViewModelBase
{
    public string PreviousPath { get; } = @"f:\git\titan\samples\titan.sandbox\assets";
    public ICommand OpenProject { get; }
    public ICommand UsePrevious { get; }
    
    public ProjectSelectorViewModel(IAppSettings config, IDialogService dialog, IMessenger messenger)
    {
        OpenProject = ReactiveCommand.CreateFromTask(async () =>
        {
            var file = await dialog.OpenFileDialog();
            if (file != null)
            {
                var path = file.EndsWith(GlobalConfiguration.ManifestFileExtension)
                    ? System.IO.Path.GetDirectoryName(file)!
                    : file;

                await messenger.SendAsync(new ProjectSelectedMessage(path));
            }
        });

        //NOTE(Jens): Hardcoded path to sandbox for now
        UsePrevious = ReactiveCommand.CreateFromTask(() => messenger.SendAsync(new ProjectSelectedMessage(PreviousPath)));
    }

    public ProjectSelectorViewModel()
    {
        if (!Design.IsDesignMode)
        {
            throw new InvalidOperationException("This constructor should only be called by the Designer.");
        }
        OpenProject = ReactiveCommand.Create(() => { });
        UsePrevious = ReactiveCommand.Create(() => { });
    }
}
