using Titan.Core;
using Titan.IOC;

namespace Titan
{
    public class Bootstrapper
    {
        public static IContainer Container { get; } = InitializeContainer();
        private static IContainer InitializeContainer()
        {
            return new Container()
                    
                    .AddRegistry<CoreRegistry>()
                
                ;

        }
    }
}
