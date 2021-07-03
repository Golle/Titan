using System.Text;
using Microsoft.CodeAnalysis;

namespace Titan.CodeGeneration
{
    [Generator]
    public class AssetTypeGenerator : ISourceGenerator
    {
        private readonly string[] _values = {"VertexShader", "PixelShader", "Shader", "Texture", "Model", "Material"};

        public void Execute(GeneratorExecutionContext context)
        {
            //var source = CreateClass();
            var source = CreateEnum();
            context.AddSource("Titan.Assets.AssetTypes", source);
        }

        private string CreateEnum()
        {
            return $@"
namespace Titan.Assets
{{
        public enum AssetTypes
        {{
            {string.Join(",",_values)},
            Count
        }}
}}
";
        }

        private string CreateClass()
        {
            var source = new StringBuilder(@"
namespace Titan.Assets
{
    public partial class AssetTypes
    {
");
            for (var i = 0; i < _values.Length; ++i)
            {
                source.Append($@"
                public const int {_values[i]} = {i + 1};
                        ");
            }

            source.Append(@"
    }
}
");
            return source.ToString();
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
