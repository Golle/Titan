using Titan.IOC;

namespace Titan
{
    internal class Bootstrapper
    {
        public static IContainer Container { get; } = InitializeContainer();
        private static IContainer InitializeContainer()
        {
            return new Container()
                
                    //.AddRegistry<>()
                
                ;

        }
    }
}
