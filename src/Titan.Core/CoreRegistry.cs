using Titan.Core.Common;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Messaging;
using Titan.Core.Threading;
using Titan.IOC;

namespace Titan.Core
{
    public class CoreRegistry : IRegistry
    {
        public void Register(IContainer container)
        {
            container
                .Register<IEventQueue, EventQueue>()
                .Register<IDateTime, DateTimeWrapper>()
                .Register<ILogFormatter, TimeLogFormatter>()
                .Register<ILog, ConsoleLogger>()
                .Register<IJsonSerializer, JsonSerializerWrapper>()
                .Register<IMemoryManager, MemoryManager>()
                .Register<WorkerPool>()
                
                .Register<FileSystem>()
                .Register<IFileReader, FileReader>()
                ;
        }
    }
}
