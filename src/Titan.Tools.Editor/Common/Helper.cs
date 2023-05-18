using Avalonia.Controls;

namespace Titan.Tools.Editor.Common;
internal static class Helper
{
    public static void CheckDesignMode(string? className = null)
    {
        if (!Design.IsDesignMode)
        {
            className ??= "n/a";
            throw new InvalidOperationException($"The constructor in class {className} should only be used by the Avalonia Designer.");
        }
    }
}
