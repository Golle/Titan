using System.Globalization;
using System.Numerics;
using Titan.Tools.Core.Models.WavefrontObj.Lexer;

namespace Titan.Tools.Core.Models.WavefrontObj;

internal class ObjParsingState
{
    public List<Vector3> Vertices { get; } = new();
    public List<Vector3> Normals { get; } = new();
    public List<Vector2> Textures { get; } = new();

    public WavefrontObject? CurrentObject;
    public WavefrontGroup? CurrentGroup;

    public List<WavefrontObject> Objects { get; } = new();
    public List<WavefrontMaterial> Materials { get; } = new();
}


public class ObjParser
{
    private static readonly Token InvalidToken = new() { Type = TokenType.Invalid };
    private readonly Token[] _tokens;
    private readonly string _basePath;

    private readonly ObjParsingState _state = new();

    private int _position;
    private Token Current => Peek(0);
    private ObjParser(Token[] tokens, string basePath)
    {
        _tokens = tokens;
        _basePath = basePath;
    }

    private Token Peek(int steps)
    {
        var position = _position + steps;
        return position < _tokens.Length ? _tokens[position] : InvalidToken;
    }

    public static WavefrontObjectResult? Load(string path)
    {
        var direcotory = Path.GetDirectoryName(path) ?? throw new InvalidOperationException("Failed to get the directory name");
        var contents = File.ReadAllText(path);
        return Parse(contents, direcotory);
    }

    public static WavefrontObjectResult? Parse(ReadOnlySpan<char> input, string basepath)
    {
        var tokens = new Tokenizer()
            .Tokenize(input)
            .Where(t => t.Type != TokenType.NewLine)
            .ToArray();
        if (tokens.Length == 0)
        {
            return null;
        }

        return new ObjParser(tokens, basepath)
            .Parse();
    }

    private WavefrontObjectResult? Parse()
    {
        var parsing = true;
        while (parsing)
        {
            var token = Current;

            switch (token.Type)
            {
                case TokenType.MaterialLib:
                    LoadMaterials();
                    break;

                case TokenType.Object:
                    ParseObject();
                    break;
                case TokenType.Group:
                    ParseGroup();
                    break;
                case TokenType.VertexNormal:
                    ParseVertexNormal();
                    break;
                case TokenType.VertexPosition:
                    ParseVertexPosition();
                    break;
                case TokenType.TextureCoordinates:
                    ParseTextureCoordinate();
                    break;
                case TokenType.Face:
                    ParseFace();
                    break;
                case TokenType.Smooth:
                    ParseSmmothing();
                    break;
                case TokenType.UseMaterial:
                    ParseUseMaterial();
                    break;
                case TokenType.EndOfFile:
                case TokenType.Invalid:
                    parsing = false;
                    break;
                default:
                    Console.Error.WriteLine($"Token {token.Type} has not been implemented.");
                    return null;
            }
        }
        return new WavefrontObjectResult(_state.Materials.ToArray(), _state.Objects.ToArray(), _state.Vertices.ToArray(), _state.Normals.ToArray(), _state.Textures.ToArray());
    }

    private WavefrontGroup GetOrCreateGroup()
    {
        if (_state.CurrentGroup == null)
        {
            var obj = GetOrCreateObject();
            _state.CurrentGroup = new WavefrontGroup(string.Empty);
            obj.Groups.Add(_state.CurrentGroup);
        }
        return _state.CurrentGroup;
    }

    private WavefrontObject GetOrCreateObject()
    {
        if (_state.CurrentObject == null)
        {
            _state.CurrentObject = new WavefrontObject(string.Empty);
            _state.Objects.Add(_state.CurrentObject);
        }

        return _state.CurrentObject;
    }

    private void ParseUseMaterial()
    {
        _position++;
        var group = GetOrCreateGroup();
        group.MaterialIndex = FindMaterial(Current.Value);
        _position++;
    }

    private int FindMaterial(string name)
    {
        for (var i = 0; i < _state.Materials.Count; ++i)
        {
            if (_state.Materials[i].Name == name)
            {
                return i;
            }
        }
        Console.Error.WriteLine($"Failed to find the material with name '{name}'");
        return -1;
    }


    private void ParseSmmothing()
    {
        _position++;
        var group = GetOrCreateGroup();
        if (Current.Type is TokenType.Number)
        {
            group.Smoothing = int.Parse(Current.Value);
        }
        else if (Current.Type is TokenType.Identifier && Current.Value == "off")
        {
            group.Smoothing = 0;
        }
        else
        {
            throw new InvalidOperationException($"The smoothing type {Current.Type} with value '{Current.Value}' is not supported.");
        }
        _position++;
    }

    private void ParseFace()
    {
        _position++;
        Span<(int Position, int Texture, int Normal)> faces = stackalloc (int, int, int)[10];

        var faceCount = 0;
        while (Current.Type is TokenType.Number)
        {
            ref var face = ref faces[faceCount++];
            face = (-1, -1, -1);
            // Read position and move forward
            face.Position = int.Parse(Current.Value) - 1;
            _position++;
            if (Current.Type != TokenType.Slash)
            {
                continue;
            }

            // Read texture and move forward
            _position++;
            if (Current.Type is TokenType.Number)
            {
                face.Texture = int.Parse(Current.Value) - 1;
                _position++;
            }
            if (Current.Type != TokenType.Slash)
            {
                continue;
            }

            // Read normal and move forward
            _position++;
            if (Current.Type is TokenType.Number)
            {
                face.Normal = int.Parse(Current.Value) - 1;
                _position++;
            }
        }

        var group = GetOrCreateGroup();
        for (var i = 2; i < faceCount; ++i)
        {
            for (var j = i - 2; j <= i; j++)
            {
                ref readonly var t1 = ref faces[j];
                group.VertexIndices.Add(t1.Position);
                group.TextureIndices.Add(t1.Texture);
                group.NormalIndices.Add(t1.Normal);
            }
        }
    }

    private void ParseTextureCoordinate()
    {
        _position++;
        _state.Textures.Add(ParseVector2());
    }

    private void ParseVertexPosition()
    {
        _position++;
        _state.Vertices.Add(ParseVector3());

    }

    private void ParseVertexNormal()
    {
        _position++;
        _state.Normals.Add(ParseVector3());
    }

    private void ParseGroup()
    {
        _position++;
        var name = Current.Value;
        _position++;

        _state.CurrentGroup = new WavefrontGroup(name);
        var obj = GetOrCreateObject();
        obj.Groups.Add(_state.CurrentGroup);
    }

    private void ParseObject()
    {
        _position++;
        var name = Current.Value;
        _position++;

        _state.CurrentObject = new WavefrontObject(name);
        _state.CurrentGroup = null; // reset the group when a new object is encountered.
        _state.Objects.Add(_state.CurrentObject);
    }



    private void LoadMaterials()
    {
        _position++;
        var relativePath = Current.Value;
        var absolutePath = Path.Combine(_basePath, relativePath);
        var materials = MtlParser.Load(absolutePath);
        _state.Materials.AddRange(materials);
        _position++;
    }

    private Vector2 ParseVector2()
    {
        var x = ParseFloat();
        var y = ParseFloat();
        return new(x, y);
    }
    private Vector3 ParseVector3()
    {
        var x = ParseFloat();
        var y = ParseFloat();
        var z = ParseFloat();
        return new(x, y, z);
    }
    private float ParseFloat()
    {
        var negative = false;
        if (Current.Type == TokenType.Minus)
        {
            negative = true;
            _position++;
        }
        if (Current.Type != TokenType.Number)
        {
            throw new InvalidOperationException($"Expected type {TokenType.Number}, got {Current.Type} Reference: {Current.Line}:{Current.Column}");
        }
        if (!float.TryParse(Current.Value, CultureInfo.InvariantCulture, out var value))
        {
            throw new InvalidOperationException($"Failed to parse the value '{Current.Value}' to a float.");
        }
        _position++;
        return negative ? -value : value;
    }
}

