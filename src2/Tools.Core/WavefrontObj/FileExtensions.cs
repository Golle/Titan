using System;
using System.IO;
using System.Threading.Tasks;

namespace Tools.Core.WavefrontObj
{
    public static class FileExtensions
    {
        public static async Task ReadToEnd(this FileStream file, Func<string, Task> callback)
        {
            using var reader = new StreamReader(file);
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                await callback(line);
            }
        }
    }
}
