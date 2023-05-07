namespace Titan.Tools.Editor.ProjectGeneration.CSharp;

internal interface ISolutionFileGenerator
{
    string GenerateSolutionFile(string projectName, string titanBasePath);


}
