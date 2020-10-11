using System.Text;

namespace Titan.Core
{
    public static class StringExtensions
    {
        public static byte[] AsBytes(this string s) => Encoding.ASCII.GetBytes(s);
    }
}
