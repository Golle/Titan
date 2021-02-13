using System.Text;

namespace Titan.Core.IO
{
    public interface IFileReader
    {
        string ReadText(string identifier);
        string ReadText(string identifier, Encoding encoding);
        string[] ReadLines(string identifier);
    }
}
