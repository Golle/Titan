using Microsoft.CodeAnalysis;

namespace Titan.SourceGenerators
{
    [Generator]
    public class HandlerGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // TODO: add implementatin for Handles when I understand how this works :O
        }
    }
}
