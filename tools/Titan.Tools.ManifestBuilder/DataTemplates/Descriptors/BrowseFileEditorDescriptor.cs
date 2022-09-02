using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using ReactiveUI;

namespace Titan.Tools.ManifestBuilder.DataTemplates.Descriptors;

internal class BrowseFileEditorDescriptor : EditorPropertyDescriptor
{
    public string? Watermark { get; set; }

    public override IControl Build()
    {
        var panel = new StackPanel { Orientation = Orientation.Horizontal };

        var textBox = new TextBox
        {
            Watermark = Watermark,
            [!TextBox.TextProperty] = new Binding(PropertyName, BindingMode.TwoWay)
        };

        panel.Children.Add(textBox);

        panel.Children.Add(new Button
        {
            Content = "Browse",
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
                    textBox.Text = files[0];
                }
            })
        });
        return panel;
    }
}
