using System;
using Titan.IOC;
using Titan.Windows;

namespace Titan
{
    public interface IEngine : IDisposable
    {
        void Start();
        internal IWindow Window { get; }
        internal IContainer Container { get; }
    }
}
