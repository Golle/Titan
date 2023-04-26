using System.Diagnostics;

namespace Titan.Tools.Core.Models.WavefrontObj.Lexer;

internal sealed class Tokenizer
{
    private readonly bool _skipComments;
    private readonly bool _skipWhitespace;

    public Tokenizer(bool skipComments = true, bool skipWhitespace = true)
    {
        _skipComments = skipComments;
        _skipWhitespace = skipWhitespace;
    }

    public IReadOnlyList<Token> Tokenize(ReadOnlySpan<char> input)
    {
        if (input.Length == 0)
        {
            return Array.Empty<Token>();
        }

        Cursor cursor = new(input);
        List<Token> tokens = new(100_000);
        do
        {
            var token = new Token(cursor.Line, cursor.Column);

            switch (cursor.Current)
            {
                case '\r':
                    // Completely ignore \r
                    continue;
                case '#':
                    Comment(ref cursor, ref token);
                    break;
                case '/':
                    token.Type = TokenType.Slash;
                    token.Value = cursor.Current.ToString();
                    break;
                case '-':
                    token.Type = TokenType.Minus;
                    token.Value = cursor.Current.ToString();
                    break;
                case '\n':
                    token.Type = TokenType.NewLine;
                    break;
                case >= '0' and <= '9':
                    NumberLiteral(ref cursor, ref token);
                    break;
                case ' ':
                    token.Type = TokenType.Whitespace;
                    token.Value = string.Empty;
                    break;
                case '\t' or '\f':
                    Debug.Assert(false, "Verify that this is not data we care about.");
                    break;
                default:
                    Identifier(ref cursor, ref token);
                    break;
            }

            if (_skipComments && token.Type == TokenType.Comment)
            {
                continue;
            }

            if (_skipWhitespace && token.Type == TokenType.Whitespace)
            {
                continue;
            }
            if (token.Type != TokenType.Unknown) // Ignore space, tabs etc that does't affect the code
            {
                tokens.Add(token);
            }
        } while (cursor.Advance());

        tokens.Add(new Token(cursor.Line, cursor.Column)
        {
            Type = TokenType.EndOfFile
        });
        return tokens;
    }


    private static void NumberLiteral(ref Cursor cursor, ref Token token)
    {
        static bool IsNumber(char c) => c is >= '0' and <= '9';
        static bool IsValidNumberLiteral(char c) => c is '.' || IsNumber(c);
        Debug.Assert(IsNumber(cursor.Current), "First character must be a number.");
        Span<char> buffer = stackalloc char[128];
        var i = 0;
        buffer[i++] = cursor.Current;
        while (IsValidNumberLiteral(cursor.Peek()))
        {
            cursor.Advance();
            buffer[i++] = cursor.Current;
        }

        token.Type = TokenType.Number;
        token.Value = new string(buffer[..i]);
    }


    private static void Identifier(ref Cursor cursor, ref Token token)
    {
        static bool IsCharacter(char c) => (c is >= 'a' and <= 'z') || (c is >= 'A' and <= 'Z');
        static bool IsNumber(char c) => c is >= '0' and <= '9';
        static bool IsSpecial(char c) => c is '_' or '.'; //NOTE(Jens): we treat a dot as an identifier. this will make it easier.
        static bool IsValidIdentifier(char c) => IsSpecial(c) || IsCharacter(c) || IsNumber(c); 
        if (!IsCharacter(cursor.Current))
        {
            throw new InvalidOperationException("Identifier must start with a character");
        }

        const int maxIdentifierSize = 256;
        Span<char> identifier = stackalloc char[maxIdentifierSize]; // TODO: verify identifers, if its longer than 256 characters increase this
        var i = 0;
        identifier[i++] = cursor.Current;
        while (IsValidIdentifier(cursor.Peek()))
        {
            cursor.Advance();
            identifier[i++] = cursor.Current;
            if (i >= maxIdentifierSize)
            {
                throw new NotSupportedException("Max Identifer size reached.");
            }
        }
        var span = identifier[..i];
        //NOTE(Jens): add detection for the types we have(vertex, normal, face etc)
        if (BuiltInTypes.TryGetType(span, out var type))
        {
            token.Type = type.Type;
            token.Value = type.Value;
        }
        else
        {
            token.Type = token.Type = TokenType.Identifier;
            token.Value = new string(span);
        }
    }

    private static void Comment(ref Cursor cursor, ref Token token)
    {
        Debug.Assert(cursor.Current == '#');
        token.Type = TokenType.Comment;
        while (cursor.Current is not '\n')
        {
            if (!cursor.Advance())
            {
                // End of file reached.
                return;
            }
        }
    }
}