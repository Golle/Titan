using Titan.Core;
using Titan.Graphics;
using Titan.Input;
using Titan.IOC;
using Titan.Windows;

namespace Titan
{
    public class Bootstrapper
    {
        public static IContainer CreateContainer() => new Container()
            .AddRegistry<CoreRegistry>()
            .AddRegistry<WindowsRegistry>()
            .AddRegistry<GraphicsRegistry>()
        
        
        
            .Register<IInputHandler, InputHandler>()
            ;
    }
}
