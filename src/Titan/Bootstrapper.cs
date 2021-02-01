using Titan.Core;
using Titan.Graphics;
using Titan.Input;
using Titan.IOC;
using Titan.Windows;

namespace Titan
{
    internal class Bootstrapper
    {
        public static IContainer CreateContainer() => new Container()
            .AddRegistry<CoreRegistry>()
            .AddRegistry<WindowsRegistry>()
            .AddRegistry<GraphicsRegistry>()
            .AddRegistry<Titan.GraphicsV2.GraphicsRegistry>()
        
        
            .Register<IInputHandler, InputHandler>()
            ;
    }
}
