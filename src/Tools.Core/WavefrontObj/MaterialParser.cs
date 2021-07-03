using System;
using System.IO;
using System.Threading.Tasks;
using Titan.Core.Logging;

namespace Tools.Core.WavefrontObj
{
    internal class MaterialParser
    {
        public async Task<ObjMaterial[]> ReadFromFile(string materialFile)
        {
            await using var file = File.OpenRead(materialFile);

            var builder = new MaterialBuilder();

            await file.ReadToEnd(line =>
            {
                if (!line.StartsWith("#") && line.Length > 0)
                {
                    ParseMaterial(builder, line);
                }
                return Task.CompletedTask;
            });
            return builder.Build();
        }

        private static void ParseMaterial(MaterialBuilder builder, in string line)
        {
            var splitLine = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var command = splitLine[0];
            var values = new ReadOnlySpan<string>(splitLine, 1, splitLine.Length - 1);
            switch (command)
            {
                case "newmtl": builder.NewMaterial(values); break;
                case "Ns": builder.SetShininess(values);break;
                case "d": builder.SetAlpha(values); break;
                case "Tr": builder.SetTransparency(values); break;
                case "illum": builder.SetIllumination(values); break;
                case "Ka": builder.SetAmbientColor(values); break;
                case "Kd": builder.SetDiffuseColor(values); break;
                case "Ks": builder.SetSpecularColor(values); break;
                case "Ke": builder.SetEmissiveColor(values); break;

                case "map_Ka": builder.SetAmbientMap(values); break;
                case "map_Kd": builder.SetDiffuseMap(values); break;
                case "map_d": builder.SetAlphaMap(values); break;
                case "map_bump": builder.SetBumpMap(values); break;
                case "bump": builder.SetBumpMap(values); break;

                // Ignored values
                case "Ni": break;
                case "Tf": break;
                default: Logger.Warning<MaterialParser>($"{command} is not recognized as a material identifier"); break;
            }
        }
    }
}
