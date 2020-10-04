using System;
using Titan.Core.Logging;

namespace Titan.Sandbox
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Container.GetInstance<ILog>()
                .Debug("apa");

            Bootstrapper.Container.GetInstance<ILog>()
                .Error("apan igen");

            Console.WriteLine("Hello Titan!");
        }
    }
}
