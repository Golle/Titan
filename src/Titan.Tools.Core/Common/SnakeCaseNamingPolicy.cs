using System.Text.Json;

namespace Titan.Tools.Core.Common;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        Span<char> buffer = stackalloc char[name.Length * 2];
        var charCount = 0;

        for (var i = 0; i < name.Length; i++)
        {
            var character = name[i];
            if (i != 0 && char.IsUpper(character))
            {
                buffer[charCount++] = '_';
            }

            buffer[charCount++] = char.ToLower(character);
        }

        return new string(buffer[..charCount]);
    }
}