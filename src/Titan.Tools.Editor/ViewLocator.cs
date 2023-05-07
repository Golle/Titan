using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Titan.Tools.Editor.ViewModels;

namespace Titan.Tools.Editor;
public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        var name = data?.GetType().FullName?.Replace("ViewModel", "View");
        if (name != null)
        {
            var type = Type.GetType(name);
            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
        }
        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
        => data is ViewModelBase;
}
