namespace Titan.Tools.Core.Models.WavefrontObj.Lexer;

internal ref struct Cursor
{
    public const char InvalidCharacter = char.MaxValue;
    private readonly ReadOnlySpan<char> _input;
    private readonly int _length;
    private int _position;
    public char Current => _input[_position];
    public int Line { get; private set; }
    public int Column { get; private set; }
    public Cursor(ReadOnlySpan<char> input)
    {
        _input = input;
        _length = _input.Length;
        _position = 0;
        Line = Column = 1;
    }

    public char Peek(int offset = 1)
    {
        if (_position + offset >= _length)
        {
            return InvalidCharacter;
        }
        return _input[_position + offset];
    }

    public bool Advance(int count)
    {
        for (var i = 0; i < count; ++i)
        {
            if (!Advance())
            {
                return false;
            }
        }

        return true;
    }

    public bool Advance()
    {
        if (_position + 1 >= _length)
        {
            return false;
        }
        var current = Current;
        if (current == '\n')
        {
            Line++;
            Column = 1;
        }
        else
        {
            Column++;
        }
        _position += 1;
        return true;
    }
}