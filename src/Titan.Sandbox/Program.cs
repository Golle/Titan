using System;
using Titan.Core.Logging;
using Titan.Windows;

namespace Titan.Sandbox
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var window = Bootstrapper.Container.GetInstance<IWindowFactory>()
                .Create(1024, 768, "Donkey box returns!");

            while (window.Update())
            {
                
                
                GC.Collect();
            }
            Console.WriteLine("Hello Titan!");
        }
    }
}
