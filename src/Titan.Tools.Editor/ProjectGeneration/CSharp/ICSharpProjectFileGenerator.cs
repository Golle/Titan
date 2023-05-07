namespace Titan.Tools.Editor.ProjectGeneration.CSharp;

internal interface ICSharpProjectFileGenerator
{
    public string GenerateProjectFileContents(params string[] projectReferences);
}
