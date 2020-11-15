using Titan.Core.Common;
using Titan.Core.Logging;
using Titan.IOC;

namespace Titan.Core
{
    public class CoreRegistry : IRegistry
    {
        public void Register(IContainer container)
        {
            container
                .Register<IDateTime, DateTimeWrapper>()
                .Register<ILogFormatter, TimeLogFormatter>()
                .Register<ILog, ConsoleLogger>()
                .Register<IFileReader, FileReader>()
                .Register<IJsonSerializer, JsonSerializerWrapper>()
                ;
        }
    }
}
