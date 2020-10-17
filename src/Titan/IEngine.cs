using System;
using Titan.Graphics.D3D11;
using Titan.IOC;
using Titan.Windows;

namespace Titan
{
    public interface IEngine : IDisposable
    {
        void Start();
        internal IGraphicsDevice Device { get; }
        internal IWindow Window { get; }
        internal IContainer Container { get; }
    }
}
