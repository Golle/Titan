namespace Titan.Core.IO
{
    public interface IFileReader
    {
        string ReadText(string identifier);
        string[] ReadLines(string identifier);
    }
}
