using System.Diagnostics;

namespace Titan.Tools.Core.Models.WavefrontObj.Lexer;


[DebuggerDisplay("{Line}:{Column}: {ToString()}")]
internal struct Token
{
    public readonly int Line;
    public readonly int Column;
    public TokenType Type;
    public string Value;

    public Token(int line, int column)
    {
        Line = line;
        Column = column;
        Type = TokenType.Unknown;
        Value = string.Empty;
    }

    public override string ToString()
    {
        if (Value == string.Empty)
        {
            return Type.ToString();
        }
        return $"{Type}:{Value}";
    }
}