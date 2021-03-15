using System;

namespace Titan.BundleBuilder
{
    internal static class Logger
    {
        public static void Info(string message) => Console.WriteLine($"[INFO] {message}");
        public static void Error(string message) => Console.Error.WriteLine($"[ERROR] {message}");
        public static void Warning(string message) => Console.Error.WriteLine($"[WARNING] {message}");
    }
}
