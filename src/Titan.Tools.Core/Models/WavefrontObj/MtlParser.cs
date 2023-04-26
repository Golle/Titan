using System.Diagnostics;
using System.Globalization;
using System.Text;
using Titan.Core.Maths;
using Titan.Tools.Core.Models.WavefrontObj.Lexer;

namespace Titan.Tools.Core.Models.WavefrontObj;

internal class MtlParser
{
    private static readonly Token InvalidToken = new() { Type = TokenType.Invalid };
    private readonly Token[] _tokens;
    private int _position;
    private Token Current => Peek(0);
    private MtlParser(Token[] tokens)
    {
        _tokens = tokens;
    }

    private Token Peek(int steps)
    {
        var position = _position + steps;
        return position < _tokens.Length ? _tokens[position] : InvalidToken;
    }

    public static WavefrontMaterial[] Load(string path)
    {
        var contents = File.ReadAllText(path);
        return Parse(contents);
    }
    public static WavefrontMaterial[] Parse(ReadOnlySpan<char> input)
    {
        var tokens = new Tokenizer()
            .Tokenize(input)
            .Where(t => t.Type != TokenType.NewLine)
            .ToArray();
        if (tokens.Length == 0)
        {
            return Array.Empty<WavefrontMaterial>();
        }

        return new MtlParser(tokens)
            .Parse();
    }


    private WavefrontMaterial[] Parse()
    {
        List<WavefrontMaterial> materials = new();
        var parsing = true;
        while (parsing)
        {
            var token = Current;

            switch (token.Type)
            {
                case TokenType.EndOfFile:
                case TokenType.Invalid:
                    parsing = false;
                    break;
                default:
                    materials.Add(ParseMaterial());
                    break;
            }
        }
        return materials.ToArray();
    }

    private WavefrontMaterial ParseMaterial()
    {
        if (Current.Type is TokenType.NewMaterial)
        {
            return NewMaterial();

        }
        throw new NotImplementedException($"The token {Current.Type} is not implemented.");
    }
    private WavefrontMaterial NewMaterial()
    {
        _position++;
        var name = Current.Type == TokenType.Identifier ? Current.Value : string.Empty;
        _position++;
        var material = new WavefrontMaterial(name);
        //NOTE(Jens): Default values for colors etc found here https://people.sc.fsu.edu/~jburkardt/data/mtl/mtl.html
        var parsing = true;
        while (parsing)
        {
            switch (Current.Type)
            {
                case TokenType.SpecularExponent:
                    _position++;
                    material = material with { SpecularExponent = ParseFloat() };
                    break;
                case TokenType.AmbientColor:
                    material = material with { Ambient = ParseColor() };
                    break;
                case TokenType.DiffuseColor:
                    material = material with { Diffuse = ParseColor() };
                    break;
                case TokenType.SpecularColor:
                    material = material with { Specular = ParseColor() };
                    break;
                case TokenType.EmissiveColor:
                    material = material with { Emissive = ParseColor() };
                    break;
                case TokenType.OpticalDensity:
                    _position++;
                    material = material with { OpticalDensity = ParseFloat() };
                    break;
                case TokenType.Illumination:
                    _position++;
                    material = material with { Illumination = ParseInt() };
                    break;
                case TokenType.TransparentInverted:
                    //NOTE(Jens): this is inverted, not sure how we should do this? 1- value might be fine 
                    _position++;
                    var transparent = ParseFloat();
                    Debug.Assert(transparent is >= 0 and <= 1);
                    material = material with { Alpha = 1.0f - transparent };
                    break;
                case TokenType.Transparent:
                    _position++;
                    material = material with { Alpha = ParseFloat() };
                    break;
                case TokenType.DiffuseTexture:
                    material = material with { DiffuseTexture = ParsePath() };
                    break;
                case TokenType.AmbientTexture:
                    material = material with { AmbientTexture = ParsePath() };
                    break;
                case TokenType.SpecularTexture:
                    material = material with { SpecularTexture = ParsePath() };
                    break;
                case TokenType.DisplacementTexture:
                    material = material with { DisplacementTexture = ParsePath() };
                    break;
                case TokenType.NewMaterial:
                case TokenType.EndOfFile:
                case TokenType.Invalid:
                    // end reached for this material/file
                    parsing = false;
                    break;
                default:
                    throw new NotImplementedException($"Token type {Current.Type} is not implemented.");
            }
        }

        return material;
    }

    private Color ParseColor()
    {
        _position++;
        Span<float> colors = stackalloc float[4];
        var count = 0;
        while (Current.Type is TokenType.Number)
        {
            colors[count++] = ParseFloat();
        }
        return Color.From(colors[..count]);
    }

    private float ParseFloat()
    {
        if (Current.Type != TokenType.Number)
        {
            throw new InvalidOperationException($"Expected type {TokenType.Number}, got {Current.Type} Reference: {Current.Line}:{Current.Column}");
        }

        if (!float.TryParse(Current.Value, CultureInfo.InvariantCulture, out var value))
        {
            throw new InvalidOperationException($"Failed to parse the value '{Current.Value}' to a float.");
        }
        _position++;
        return value;
    }

    private int ParseInt()
    {
        if (Current.Type != TokenType.Number)
        {
            throw new InvalidOperationException($"Expected type {TokenType.Number}, got {Current.Type} Reference: {Current.Line}:{Current.Column}");
        }

        if (!int.TryParse(Current.Value, out var value))
        {
            throw new InvalidOperationException($"Failed to parse the value '{Current.Value}' to an int.");
        }
        _position++;
        return value;
    }

    private string ParsePath()
    {
        _position++;
        StringBuilder builder = new();
        while (Current.Type is TokenType.Identifier or TokenType.Slash)
        {
            builder.Append(Current.Value);

            _position++;
        }
        return builder.ToString();
    }
}
