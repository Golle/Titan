using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using ReactiveUI;
using Titan.Tools.ManifestBuilder.DataTemplates.Controls;

namespace Titan.Tools.ManifestBuilder.DataTemplates.Descriptors;

internal class BrowseFileEditorDescriptor : EditorPropertyDescriptor
{
    public string? Watermark { get; set; }
    public string? ButtonText { get; set; }

    public override IControl Build()
    {
        var panel = new StackPanel
        {
            Spacing = 10,
            Children = { new TextBlock { Text = Name }}
        };

        if (Description != null)
        {
            panel.Children.Add(new DescriptionTextBlock(Description));
        }

        var fileTextBox = new TextBox
        {
            Watermark = Watermark,
            [!TextBox.TextProperty] = new Binding(PropertyName, BindingMode.TwoWay),
            [Grid.ColumnProperty] = 0
        };

        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*, 10, Auto"),
            Children =
            {
                new Button
                {
                    Content = ButtonText ?? "Browse",
                    Command = ReactiveCommand.CreateFromTask(async () =>
                    {
                        var dialog = new OpenFileDialog
                        {
                            AllowMultiple = false,
                            Title = "Select file"
                        };
                        var files = await dialog.ShowAsync(App.MainWindow); //NOTE(Jens): Maybe we want the current window?
                        if (files != null && files.Length != 0)
                        {
                            fileTextBox.Text = files[0];
                        }
                    }),
                    [Grid.ColumnProperty] = 2
                }
            }
        };
        //var dockPanel = new DockPanel
        //{
        //    LastChildFill = true,
        //    Children =
        //    {
        //        new Button
        //        {
        //            Content = ButtonText ?? "Browse",
        //            Command = ReactiveCommand.CreateFromTask(async () =>
        //            {
        //                var dialog = new OpenFileDialog
        //                {
        //                    AllowMultiple = false,
        //                    Title = "Select file"
        //                };
        //                var files = await dialog.ShowAsync(App.MainWindow); //NOTE(Jens): Maybe we want the current window?
        //                if (files != null && files.Length != 0)
        //                {
        //                    fileTextBox.Text = files[0];
        //                }
        //            }),
        //            [DockPanel.DockProperty] = Dock.Right
        //        }
        //    }
        //};
        //dockPanel.Children.Add(fileTextBox);
        grid.Children.Add(fileTextBox);
        panel.Children.Add(grid);
        return panel;
    }
}
