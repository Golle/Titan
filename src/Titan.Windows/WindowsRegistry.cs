using Titan.IOC;

namespace Titan.Windows
{
    public class WindowsRegistry : IRegistry
    {
        public void Register(IContainer container)
        {
            container
                .Register<IWindowFactory, Win32WindowFactory>()

                ;
        }
    }
}
