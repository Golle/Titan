using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Titan.Tools.Editor.ProjectGeneration.CSharp;

internal class CSharpProjectFileGenerator : ICSharpProjectFileGenerator
{
    public string GenerateProjectFileContents(params string[] projectReferences)
    {
        var projectFile = new XDocument(new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk")));
        var root = projectFile.Root ?? throw new InvalidOperationException("Root is null, what happened?");

        // base properties
        {
            var propertyGroup = new XElement("PropertyGroup");
            propertyGroup.Add(new XElement("TargetFramework", "net8.0"));
            propertyGroup.Add(new XElement("ImplicitUsings", "true"));
            propertyGroup.Add(new XElement("PlatformTarget", "x64"));
            propertyGroup.Add(new XElement("Nullable", "disable"));
            propertyGroup.Add(new XElement("ConsoleLogging", "true"));
            propertyGroup.Add(new XElement("ConsoleWindow", "true"));
            root.Add(propertyGroup);
        }

        // project reference

        if (projectReferences.Length > 0)
        {
            var itemGroup = new XElement("ItemGroup");
            foreach (var projectReference in projectReferences)
            {
                itemGroup.Add(new XElement("ProjectReference", new XAttribute("Include", projectReference)));
            }
            root.Add(itemGroup);
        }
        // Console window
        {
            var propertyGroup = new XElement("PropertyGroup", new XAttribute("Condition", "'$(ConsoleWindow)' == 'false'"));
            propertyGroup.Add(new XElement("OutputType", "WinExe"));
            root.Add(propertyGroup);
        }
        {
            var propertyGroup = new XElement("PropertyGroup", new XAttribute("Condition", "'$(ConsoleWindow)' == 'true'"));
            propertyGroup.Add(new XElement("OutputType", "Exe"));
            root.Add(propertyGroup);
        }

        // Console logging
        {
            var propertyGroup = new XElement("PropertyGroup", new XAttribute("Condition", "'$(ConsoleLogging)' == 'true'"));
            propertyGroup.Add(new XElement("DefineConstants", "$(DefineConstants);CONSOLE_LOGGING"));
            root.Add(propertyGroup);
        }

        var builder = new StringBuilder();
        using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true }))
        {
            projectFile.Save(writer);
        }
        return builder.ToString();
    }
}
