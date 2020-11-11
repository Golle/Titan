using System;
using Titan.IOC;
using Titan.Windows;

namespace Titan
{
    public interface IEngine : IDisposable
    {
        void Start();
        void Stop();
        internal IWindow Window { get; }
        internal IContainer Container { get; }
    }
}
